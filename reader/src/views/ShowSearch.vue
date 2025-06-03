<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import axios from 'axios'
import type { AxiosResponse } from 'axios'
import { useMangaStore } from '@/store/manga'
import router from '@/router'
import { ElMessage } from 'element-plus'
import { useRoute } from 'vue-router'
import type {
  Title,
  MangaSearchTitleResponse,
  MangaData1,
  MangaAttributes,
  MangaTag,
  TagAttributes,
  MangaRelationship,
  CoverArtAttributes1,
} from '@/components/ChapterInfo'
import { ElPagination } from 'element-plus'

const route = useRoute()

const title = route.query.title
console.log(title)
const mangaStore = useMangaStore()
const mangaList = computed(() => {
  return mangaStore.searchResultList as MangaData1[]
})

const loading = ref(true)
const totalResults = ref(0)
const currentPage = ref(1)
const limit = 10

const fetchSearchResults = async () => {
  if (!title) {
    mangaStore.setSearchResultList([])
    totalResults.value = 0
    loading.value = false
    return
  }

  console.log(
    `Fetching search results for "${title}" - Page: ${currentPage.value}, Limit: ${limit}`,
  )
  try {
    loading.value = true
    mangaStore.setSearchResultList([])

    const offset = (currentPage.value - 1) * limit

    const res: AxiosResponse<MangaSearchTitleResponse> = await axios.get(
      '/api/Mangadex/GetMangaByOptions',
      {
        params: {
          Title: title,
          Limit: limit,
          Offset: offset,
        },
      },
    )

    if (res.status === 200 && res.data && Array.isArray(res.data.data)) {
      console.log(res.data)
      mangaStore.setSearchResultList(res.data.data)
      totalResults.value = res.data.total || 0
    } else {
      mangaStore.setSearchResultList([])
      totalResults.value = 0
    }
  } catch (e) {
    console.error('axios error:', e)
    ElMessage.error('搜索失败')
    mangaStore.setSearchResultList([])
    totalResults.value = 0
  } finally {
    loading.value = false
  }
}

const handlePageChange = (newPage: number) => {
  currentPage.value = newPage
  fetchSearchResults()
}

onMounted(async () => {
  currentPage.value = 1
  fetchSearchResults()
})

function getCoverUrl(manga: MangaData1) {
  console.log(manga)
  const file = manga.relationships.find((e) => e.type === 'cover_art')
  return `https://uploads.mangadex.org/covers/${manga.id}/${file?.attributes?.fileName}`
}

function getTitle(manga: MangaData1) {
  return manga.attributes.title?.en || 'Untitled'
}

function goToComic(id: string) {
  router.push(`/comic/${id}`)
}
</script>

<template>
  <div v-if="loading" class="loading-text">正在加载中...</div>
  <div v-else-if="mangaList.length" class="manga-list">
    <div v-for="manga in mangaList" :key="manga.id" @click="goToComic(manga.id)" class="manga-card">
      <img :src="getCoverUrl(manga)" alt="cover" class="cover-img" />
      <div class="title">{{ getTitle(manga) }}</div>
    </div>
    <el-pagination
      background
      layout="total, prev, pager, next, jumper"
      :total="totalResults"
      :page-size="limit"
      v-model:current-page="currentPage"
      @current-change="handlePageChange"
      class="pagination"
    />
  </div>
  <div v-else class="empty-text">未找到相关漫画</div>
</template>

<style scoped>
.loading-text,
.empty-text {
  color: #c7cfe6;
  text-align: center;
  margin-top: 32px;
  font-size: 16px;
}

.manga-list {
  display: flex;
  flex-wrap: wrap;
  gap: 32px 36px;
  justify-content: flex-start;
  max-width: 1320px;
  margin: 20px auto;
  padding: 0 18px;
}
.manga-card {
  width: 180px;
  background: #232c43;
  border-radius: 12px;
  padding: 12px;
  color: #fff;
  display: flex;
  flex-direction: column;
  align-items: center;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.13);
  cursor: pointer;
  transition:
    transform 0.2s,
    box-shadow 0.2s;
}
.manga-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}
.cover-img {
  width: 100%;
  height: 240px;
  object-fit: cover;
  border-radius: 8px;
  margin-bottom: 10px;
  background: #222;
}
.title {
  font-size: 1.08rem;
  font-weight: 600;
  text-align: center;
  margin-top: 6px;
  color: #c7cfe6;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  width: 100%;
}
.pagination {
  margin-top: 20px;
  justify-content: center;
  width: 100%;
}
</style>
