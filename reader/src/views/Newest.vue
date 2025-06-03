<template>
  <div class="newest-page">
    <div class="manga-grid">
      <div
        v-for="manga in mangaList"
        :key="manga.id"
        class="manga-card"
        @click="goToComic(manga.id)"
      >
        <img :src="manga.cover" alt="封面" class="cover-img" />
        <p class="manga-title">{{ manga.title }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router'

const mangaList = ref<any[]>([])
const router = useRouter()

const fetchNewestManga = async () => {
  try {
    const params = {
      Order: 'updatedAt,desc',
      Limit: 20,
    }

    const res = await axios.get('/api/Mangadex/GetMangaByOptions', { params })
    const rawData = res.data?.data || []

    mangaList.value = rawData.map((item: any) => {
      const id = item.id
      const titleObj = item.attributes?.title || {}
      const title = titleObj.en || Object.values(titleObj)[0] || 'Untitled'

      const coverRel = item.relationships?.find((r: any) => r.type === 'cover_art')
      const fileName = coverRel?.attributes?.fileName
      const cover = fileName ? `https://uploads.mangadex.org/covers/${id}/${fileName}` : ''

      return { id, title, cover }
    })
  } catch (err) {
    console.error('获取最新漫画失败', err)
  }
}

const goToComic = (id: string) => {
  router.push({ name: 'Comic', params: { id } })
}

onMounted(() => {
  fetchNewestManga()
})
</script>

<style scoped>
.newest-page {
  max-width: 1320px;
  margin: 40px auto;
  padding: 0 18px;
}
.manga-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 32px 36px;
  justify-content: flex-start;
}
.manga-card {
  background-color: #1f2635;
  border-radius: 16px;
  overflow: hidden;
  cursor: pointer;
  width: 200px;
  height: 320px;
  box-shadow: 0 4px 18px rgba(0, 0, 0, 0.13);
  transition:
    transform 0.22s cubic-bezier(0.4, 2, 0.6, 1),
    box-shadow 0.22s;
  display: flex;
  flex-direction: column;
  align-items: stretch;
  padding: 0;
}
.manga-card:hover {
  transform: scale(1.045) translateY(-8px) rotateZ(-0.5deg);
  box-shadow: 0 10px 32px rgba(56, 189, 248, 0.19);
  z-index: 2;
}
.cover-img {
  width: 100%;
  height: 210px;
  object-fit: cover;
  border-radius: 16px 16px 0 0;
  background: #232c43;
  flex-shrink: 0;
}
.manga-title {
  color: #fff;
  font-size: 1.08rem;
  text-align: center;
  padding: 14px 10px 10px 10px;
  background: rgba(31, 38, 53, 0.92);
  border-radius: 0 0 16px 16px;
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 60px;
  max-height: 70px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: normal;
  word-break: break-all;
  font-weight: 600;
  line-height: 1.25;
}
</style>
