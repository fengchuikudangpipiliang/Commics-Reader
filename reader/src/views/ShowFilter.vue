<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import axios from 'axios'
import { useMangaStore } from '@/store/manga'
import type { MangaData1 } from '@/components/ChapterInfo'
import router from '@/router'
import { ElMessage } from 'element-plus'
import { useFilterStore } from '@/store/fliter'
import { useRoute } from 'vue-router'
import { ElPagination } from 'element-plus'

const mangaStore = useMangaStore()
const mangaList = computed(() => {
  return mangaStore.searchResultList as MangaData1[]
})
const filterStore = useFilterStore()
const route = useRoute()
const query = route.query
console.log(query)

const filters = filterStore.filter
const loading = ref(true)
const totalResults = ref(0)
const currentPage = ref(1)

const fetchMangaResults = async () => {
  console.log('Fetching manga results...')
  try {
    loading.value = true
    mangaStore.setSearchResultList([])

    const currentOffset = (currentPage.value - 1) * filters.limit

    const res = await axios.get('/api/Mangadex/GetMangaByOptions', {
      params: {
        Title: filters.title,
        OriginalLanguage: filters.originalLanguage,
        AvailableTranslatedLanguages: filters.translatedLanguages,
        Status: filters.status,
        ContentRatings: filters.contentRating,
        Year: filters.year,
        Limit: filters.limit,
        Offset: currentOffset,
        Order: filters.order,
        IncludedTags: filters.includedTags,
        ExcludedTags: filters.excludedTags,
      },
    })

    console.log(1)
    console.log(res.data)
    mangaStore.setSearchResultList(res.data.data)
    totalResults.value = res.data.total
  } catch (error) {
    ElMessage.error('搜索失败')
    console.error(error)
  } finally {
    loading.value = false
  }
}

const handlePageChange = (newPage: number) => {
  currentPage.value = newPage
  filterStore.setFilter({
    ...filterStore.filter,
    offset: (newPage - 1) * filters.limit,
  })
  fetchMangaResults()
}

onMounted(async () => {
  currentPage.value = 1
  filters.offset = 0
  fetchMangaResults()
})

function getCoverUrl(manga: MangaData1) {
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
  <!-- 结果渲染 -->
  <div v-if="loading" class="loading-text">正在加载中...</div>
  <div v-else-if="mangaList.length" class="manga-list">
    <div v-for="manga in mangaList" :key="manga.id" @click="goToComic(manga.id)" class="manga-card">
      <img :src="getCoverUrl(manga)" alt="cover" class="cover-img" />
      <div class="title" :title="getTitle(manga)">{{ getTitle(manga) }}</div>
    </div>
    <el-pagination
      background
      layout="total, prev, pager, next, jumper"
      :total="totalResults"
      :page-size="filters.limit"
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

.fliter-page {
  padding: 32px 0;
}
.manga-list {
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

.cover-img {
  width: 100%;
  height: 220px;
  object-fit: cover;
  border-radius: 16px 16px 0 0;
  background: #232c43;
  flex-shrink: 0;
  transition: transform 0.3s ease;
}

.manga-card:hover .cover-img {
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
  min-height: 70px;
  max-height: 70px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: normal;
  word-break: break-all;
  font-weight: 600;
  line-height: 1.3;
  margin: 0;
}

.manga-card:hover .title {
  white-space: normal;
  overflow: visible;
  text-overflow: clip;
  position: relative;
  z-index: 10;
}

.filter-bar-dark {
  display: flex;
  flex-wrap: nowrap;
  gap: 6px;
  background: #1a2035;
  padding: 7px 8px 4px 8px;
  border-radius: 10px;
  margin-bottom: 14px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  align-items: center;
  overflow-x: auto;
}
.filter-select,
.filter-input,
.filter-input-number,
.filter-search-btn {
  flex: 1 1 0;
  min-width: 0;
  max-width: 150px;
  background: #232c43;
  border-radius: 8px;
  border: 1px solid #232c43;
  color: #c7cfe6;
  font-size: 14px;
  height: 36px;
  box-sizing: border-box;
  display: flex;
  align-items: center;
}
.filter-select .el-input__inner,
.filter-input .el-input__inner {
  background: #232c43;
  color: #c7cfe6;
  border-radius: 8px;
  font-size: 14px;
  height: 36px;
  display: flex;
  align-items: center;
}
.filter-input-number .el-input__inner {
  background: #232c43;
  color: #c7cfe6;
  border-radius: 8px;
  font-size: 14px;
  height: 36px;
  display: flex;
  align-items: center;
}
.filter-select:hover,
.filter-select:focus-within,
.filter-input:hover,
.filter-input:focus-within,
.filter-input-number:hover,
.filter-input-number:focus-within {
  border-color: #82b1ff;
  box-shadow: 0 0 0 2px #2563eb33;
}
.filter-search-btn {
  min-width: 80px;
  max-width: 100px;
  height: 36px;
  background: #2563eb;
  color: #fff;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  font-size: 14px;
  margin-left: 4px;
  transition: background 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}
.filter-search-btn:hover {
  background: #1a47a1;
}

.pagination {
  margin-top: 20px;
  justify-content: center;
  width: 100%;
}
</style>
