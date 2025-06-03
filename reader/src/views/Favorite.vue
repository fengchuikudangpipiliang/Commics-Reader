<template>
  <div class="favorite-container">
    <div v-if="!userId" class="not-logged-in">您还没有登录，请登录后使用此功能。</div>
    <div v-else>
      <el-skeleton v-if="loading" rows="6" animated />
      <div v-else>
        <div v-if="mangas.length === 0">暂无收藏</div>
        <div class="manga-list">
          <div v-for="m in mangas" :key="m.id" class="manga-card">
            <img :src="getCoverUrl(m)" class="cover" @click="goToDetail(m.id)" />
            <p class="title" @click="goToDetail(m.id)">
              {{ m.attributes.title?.en || 'Untitled' }}
            </p>
            <el-button type="danger" size="small" plain @click="unFavorite(m.id)">
              取消收藏
            </el-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { useUserStore } from '@/store/login'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'

const userStore = useUserStore()
const router = useRouter()
const userId = userStore.userId
const mangaUuidList = ref<string[]>([])
const mangas = ref<any[]>([])
const loading = ref(true)

function getCoverUrl(manga: any) {
  const coverRel = manga.relationships?.find((r: any) => r.type === 'cover_art')
  const fileName = coverRel?.attributes?.fileName
  return fileName ? `https://uploads.mangadex.org/covers/${manga.id}/${fileName}` : ''
}

function goToDetail(uuid: string) {
  router.push(`/comic/${uuid}`)
}

async function unFavorite(uuid: string) {
  if (!userId) return
  try {
    await axios.post('/api/UsersLove/SubUsersLove', null, {
      params: { uuid, id: userId },
    })
    mangas.value = mangas.value.filter((m) => m.id !== uuid)
    ElMessage.success('取消收藏成功')
  } catch (err) {
    console.error('取消收藏失败', err)
    ElMessage.error('取消收藏失败')
  }
}

async function fetchMangaByUuid(uuid: string): Promise<any | null> {
  try {
    const res = await axios.get('/api/Mangadex/GetMangaById', {
      params: { uuid, limit: 100, offset: 0 },
    })
    const list = res.data.data || []
    return list.find((item: any) => item.id === uuid) || null
  } catch (err) {
    console.error(`获取漫画 ${uuid} 失败`, err)
    return null
  }
}

onMounted(async () => {
  if (!userId) return
  try {
    const res = await axios.get('/api/UsersLove/GetUsersLove', {
      params: { id: userId },
    })
    mangaUuidList.value = res.data
    const all = await Promise.all(mangaUuidList.value.map((uuid) => fetchMangaByUuid(uuid)))
    mangas.value = all.filter((m) => m !== null)
  } catch (error) {
    console.error('获取收藏数据失败：', error)
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.favorite-container {
  max-width: 1320px;
  margin: 40px auto;
  padding: 0 18px;
  font-size: 1.2rem;
  color: #ddd;
}

.not-logged-in {
  color: #f56c6c;
  text-align: center;
  font-size: 1.4rem;
  margin: 40px 0;
}

.manga-list {
  display: flex;
  flex-wrap: wrap;
  gap: 32px 36px;
  justify-content: flex-start;
  margin-top: 20px;
}

.manga-card {
  background-color: #1f2635;
  border-radius: 16px;
  overflow: hidden;
  cursor: pointer;
  width: 180px;
  height: 300px;
  box-shadow: 0 4px 18px rgba(0, 0, 0, 0.13);
  transition:
    transform 0.22s cubic-bezier(0.4, 2, 0.6, 1),
    box-shadow 0.22s;
  display: flex;
  flex-direction: column;
  align-items: stretch;
  padding: 0;
  position: relative;
}

.manga-card:hover {
  transform: scale(1.045) translateY(-8px) rotateZ(-0.5deg);
  box-shadow: 0 10px 32px rgba(56, 189, 248, 0.19);
  z-index: 2;
}

.cover {
  width: 100%;
  height: 220px;
  object-fit: cover;
  border-radius: 16px 16px 0 0;
  background: #232c43;
  flex-shrink: 0;
  transition: transform 0.3s ease;
}

.manga-card:hover .cover {
  transform: scale(1.05);
}

.title {
  color: #fff;
  font-size: 0.95rem;
  text-align: center;
  padding: 12px 10px;
  background: rgba(31, 38, 53, 0.92);
  border-radius: 0 0 16px 16px;
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 60px;
  max-height: 60px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: normal;
  word-break: break-all;
  font-weight: 600;
  line-height: 1.3;
  margin: 0;
}

:deep(.el-button) {
  position: absolute;
  bottom: 10px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 3;
  opacity: 0;
  transition: all 0.2s ease;
  padding: 6px 12px;
  font-size: 0.85rem;
}

.manga-card:hover :deep(.el-button) {
  opacity: 1;
  bottom: 14px;
}
</style>
