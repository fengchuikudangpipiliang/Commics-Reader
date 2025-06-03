<script setup lang="ts">
import { computed, onMounted, onUnmounted, reactive, ref } from 'vue'
import axios from 'axios'
import { useMangaStore } from '@/store/manga'
import type { MangaData1 } from '@/components/ChapterInfo'
import router from '@/router'
import { ElMessage } from 'element-plus'
import { useFilterStore } from '@/store/fliter'
import type { Filter } from '@/type/filter'
import { watch } from 'vue'
import { ElPopover, ElTag, ElScrollbar } from 'element-plus'

const mangaStore = useMangaStore()
const loading = ref(false)
onMounted(() => {
  console.log('被刷新了')
})

const mangaList = computed(() => {
  return mangaStore.searchResultList as MangaData1[]
})
let time = ref(123)
const filterStore = useFilterStore()
const filters = reactive<Filter>({
  title: '',
  originalLanguage: [],
  translatedLanguages: [],
  status: [],
  contentRating: [],
  year: '',
  limit: 70,
  offset: 0,
  order: '',
  includedTags: [],
  excludedTags: [],
})

const includedTagsPopoverVisible = ref(false)
const excludedTagsPopoverVisible = ref(false)

onUnmounted(() => {
  filterStore.setFilter({
    title: '',
    originalLanguage: [],
    translatedLanguages: [],
    status: [],
    contentRating: [],
    year: '',
    limit: 70,
    offset: 0,
    order: '',
    includedTags: [],
    excludedTags: [],
  })
})
watch(
  filters,
  () => {
    time.value = Date.now()
  },
  { deep: true },
)
const onSearchClick = async () => {
  filterStore.setFilter(filters)
  router.push({
    path: '/fliter/showfilter',
    query: { time: time.value.toString() },
  })
}

function getTagName(tagValue: string) {
  return tagValue
}

const includedTagsSummary = computed(() => {
  if (filters.includedTags.length === 0) {
    return 'Include Tags'
  } else if (filters.includedTags.length <= 2) {
    return filters.includedTags.map(getTagName).join(', ')
  } else {
    return `Include Tags +${filters.includedTags.length}`
  }
})

const excludedTagsSummary = computed(() => {
  if (filters.excludedTags.length === 0) {
    return 'Exclude Tags'
  } else if (filters.excludedTags.length <= 2) {
    return filters.excludedTags.map(getTagName).join(', ')
  } else {
    return `Exclude Tags +${filters.excludedTags.length}`
  }
})

const languageOptions = [
  { value: 'zh', label: 'Simplified Chinese' },
  { value: 'zh-hk', label: 'Traditional Chinese' },
  { value: 'pt-br', label: 'Brazilian Portugese' },
  { value: 'es', label: 'Castilian Spanish' },
  { value: 'es-la', label: 'Latin American Spanish' },
  { value: 'ja-ro', label: 'Romanized Japanese' },
  { value: 'ko-ro', label: 'Romanized Korean' },
  { value: 'zh-ro', label: 'Romanized Chinese' },
]
const contentRatingOptions = ['safe', 'suggest', 'erotica', 'porngraphic']
const statusOptions = ['ongoing', 'completed', 'hiatus', 'cancelled']
const orderOptions = [
  { value: 'createdAt,asc', label: 'Created At Asc' },
  { value: 'createdAt,desc', label: 'Created At Desc' },
  { value: 'updatedAt,asc', label: 'Updated At Asc' },
  { value: 'updatedAt,desc', label: 'Updated At Desc' },
]
const tagOptions = [
  'Oneshot',
  'Thriller',
  'Award Winning',
  'Reincarnation',
  'Sci-Fi',
  'Time Travel',
  'Genderswap',
  'Loli',
  'Traditional Games',
  'Official Colored',
  'Historical',
  'Monsters',
  'Action',
  'Demons',
  'Psychological',
  'Ghosts',
  'Animals',
  'Long Strip',
  'Romance',
  'Ninja',
  'Comedy',
  'Mecha',
  'Anthology',
  "Boys' Love",
  'Incest',
  'Crime',
  'Survival',
  'Zombies',
  'Reverse Harem',
  'Sports',
  'Superhero',
  'Martial Arts',
  'Fan Colored',
  'Samurai',
  'Magical Girls',
  'Mafia',
  'Adventure',
  'Self-Published',
  'Virtual Reality',
  'Office Workers',
  'Video Games',
  'Post-Apocalyptic',
  'Sexual Violence',
  'Crossdressing',
  'Magic',
  "Girls' Love",
  'Harem',
  'Military',
  'Wuxia',
  'Isekai',
  '4-Koma',
  'Doujinshi',
  'Philosophical',
  'Gore',
  'Drama',
  'Medical',
  'School Life',
  'Horror',
  'Fantasy',
  'Villainess',
  'Vampires',
  'Delinquents',
  'Monster Girls',
  'Shota',
  'Police',
  'Web Comic',
  'Slice of Life',
  'Aliens',
  'Cooking',
  'Supernatural',
  'Mystery',
  'Adaptation',
  'Music',
  'Full Color',
  'Tragedy',
  'Gyaru',
]

function toggleTag(tag: string, type: 'included' | 'excluded') {
  const tagList = type === 'included' ? filters.includedTags : filters.excludedTags
  const index = tagList.indexOf(tag)

  if (index > -1) {
    tagList.splice(index, 1)
  } else {
    tagList.push(tag)
  }
}
</script>

<template>
  <div class="fliter-page">
    <div class="filter-grid">
      <div class="filter-item">
        <label class="filter-label">Title</label>
        <el-input v-model="filters.title" placeholder="Title" class="filter-input" />
      </div>

      <div class="filter-item">
        <label class="filter-label">Original Language</label>
        <el-select
          v-model="filters.originalLanguage"
          multiple
          filterable
          clearable
          placeholder="Any"
          class="filter-select"
        >
          <el-option
            v-for="item in languageOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </div>

      <div class="filter-item">
        <label class="filter-label">Translated Languages</label>
        <el-select
          v-model="filters.translatedLanguages"
          multiple
          filterable
          clearable
          placeholder="Any"
          class="filter-select"
        >
          <el-option
            v-for="item in languageOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </div>

      <div class="filter-item">
        <label class="filter-label">Status</label>
        <el-select
          v-model="filters.status"
          multiple
          filterable
          clearable
          placeholder="Any"
          class="filter-select"
        >
          <el-option v-for="item in statusOptions" :key="item" :label="item" :value="item" />
        </el-select>
      </div>

      <div class="filter-item">
        <label class="filter-label">Content Rating</label>
        <el-select
          v-model="filters.contentRating"
          multiple
          filterable
          clearable
          placeholder="Any"
          class="filter-select"
        >
          <el-option v-for="item in contentRatingOptions" :key="item" :label="item" :value="item" />
        </el-select>
      </div>

      <div class="filter-item">
        <label class="filter-label">Order</label>
        <el-select
          v-model="filters.order"
          filterable
          clearable
          placeholder="None"
          class="filter-select"
        >
          <el-option
            v-for="item in orderOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </div>

      <div class="filter-item">
        <label class="filter-label">Publication Year</label>
        <el-input v-model="filters.year" placeholder="Any" class="filter-input" />
      </div>

      <div class="filter-item filter-item-tags">
        <label class="filter-label"
          >Included Tags
          {{ filters.includedTags.length > 0 ? `+${filters.includedTags.length}` : '' }}</label
        >
        <el-popover
          placement="bottom-start"
          :width="400"
          trigger="click"
          v-model:visible="includedTagsPopoverVisible"
        >
          <template #reference>
            <div class="filter-select filter-tag-trigger-text">
              {{
                includedTagsSummary.startsWith('Include Tags')
                  ? includedTagsSummary.replace('Include Tags', '').trim()
                  : includedTagsSummary
              }}
              <span v-if="includedTagsSummary === 'Include Tags'" class="placeholder-text"
                >Any</span
              >
            </div>
          </template>
          <div class="tag-selection-content">
            <h4>Include Tags</h4>
            <el-scrollbar max-height="200px">
              <div class="tag-list-grid">
                <el-tag
                  v-for="tag in tagOptions"
                  :key="tag"
                  :type="filters.includedTags.includes(tag) ? 'primary' : 'info'"
                  :effect="filters.includedTags.includes(tag) ? 'dark' : 'light'"
                  @click="toggleTag(tag, 'included')"
                  class="tag-item"
                  size="large"
                >
                  {{ tag }}
                </el-tag>
              </div>
            </el-scrollbar>
          </div>
        </el-popover>
      </div>

      <div class="filter-item filter-item-tags">
        <label class="filter-label"
          >Excluded Tags
          {{ filters.excludedTags.length > 0 ? `+${filters.excludedTags.length}` : '' }}</label
        >
        <el-popover
          placement="bottom-start"
          :width="400"
          trigger="click"
          v-model:visible="excludedTagsPopoverVisible"
        >
          <template #reference>
            <div class="filter-select filter-tag-trigger-text">
              {{
                excludedTagsSummary.startsWith('Exclude Tags')
                  ? excludedTagsSummary.replace('Exclude Tags', '').trim()
                  : excludedTagsSummary
              }}
              <span v-if="excludedTagsSummary === 'Exclude Tags'" class="placeholder-text"
                >Any</span
              >
            </div>
          </template>
          <div class="tag-selection-content">
            <h4>Exclude Tags</h4>
            <el-scrollbar max-height="200px">
              <div class="tag-list-grid">
                <el-tag
                  v-for="tag in tagOptions"
                  :key="tag"
                  :type="filters.excludedTags.includes(tag) ? 'danger' : 'info'"
                  :effect="filters.excludedTags.includes(tag) ? 'dark' : 'light'"
                  @click="toggleTag(tag, 'excluded')"
                  class="tag-item"
                  size="large"
                >
                  {{ tag }}
                </el-tag>
              </div>
            </el-scrollbar>
          </div>
        </el-popover>
      </div>

      <div class="filter-item">
        <label class="filter-label">&nbsp;</label>
        <el-button class="filter-search-btn" type="primary" @click="onSearchClick"
          >Search</el-button
        >
      </div>
    </div>
  </div>
  <RouterView :key="$route.fullPath"></RouterView>
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
  padding: 32px 18px;
}
.manga-list {
  display: flex;
  flex-wrap: wrap;
  gap: 32px;
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
}
.filter-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
  gap: 16px 20px;
  background: #1a2035;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  align-items: start;
}
.filter-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.filter-label {
  color: #c7cfe6;
  font-size: 0.95rem;
  font-weight: 500;
  margin-bottom: 0;
}
.filter-input,
.filter-select,
.filter-tag-trigger-text {
  width: 100%;
  height: 38px;
  background: #eef1f4;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  color: #606266;
  font-size: 14px;
  box-sizing: border-box;
  padding: 0 12px;
  display: flex;
  align-items: center;
  cursor: pointer;
}
.filter-tag-trigger-text .placeholder-text {
  color: #a8abb2;
}
.filter-input:hover,
.filter-select:hover,
.filter-tag-trigger-text:hover {
  border-color: #c0c4cc;
}
.filter-input:focus-within,
.filter-select:focus-within,
.filter-tag-trigger-text:focus {
  border-color: #409eff;
  box-shadow: 0 0 0 1px #409eff;
  outline: none;
}
.filter-item .filter-search-btn {
  width: 100%;
  height: 38px;
  margin-left: 0;
  background: #2563eb;
  color: #fff;
  border: none;
  border-radius: 4px;
  font-weight: 600;
  font-size: 14px;
  transition: background 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}
.filter-item .filter-search-btn:hover {
  background: #1a47a1;
}
.tag-selection-content h4 {
  margin-top: 0;
  margin-bottom: 10px;
  color: #303133;
  font-size: 1rem;
}
.tag-list-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 8px;
}
.tag-item {
  cursor: pointer;
  transition:
    background-color 0.2s,
    border-color 0.2s;
}
.tag-item:hover {
  opacity: 0.8;
}
:deep(.el-scrollbar__wrap) {
  margin-right: -10px !important;
}
:deep(.el-popover) {
  background-color: #fff !important;
  color: #303133 !important;
  border-color: #dcdfe6 !important;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.12) !important;
}
</style>
