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
  { value: 'zh', label: '简体中文' },
  { value: 'zh-hk', label: '繁体中文' },
  { value: 'pt-br', label: '巴西葡萄牙语' },
  { value: 'es', label: '西班牙语' },
  { value: 'es-la', label: '拉丁美洲西班牙语' },
  { value: 'ja-ro', label: '罗马化日语' },
  { value: 'ko-ro', label: '罗马化韩语' },
  { value: 'zh-ro', label: '罗马化中文' },
]
const contentRatingOptions = ['安全', '建议', '情色', '色情']
const statusOptions = ['连载中', '已完结', '暂停', '已取消']
const orderOptions = [
  { value: 'createdAt,asc', label: '创建时间升序' },
  { value: 'createdAt,desc', label: '创建时间降序' },
  { value: 'updatedAt,asc', label: '更新时间升序' },
  { value: 'updatedAt,desc', label: '更新时间降序' },
]
const tagOptions = [
  '单篇',
  '惊悚',
  '获奖作品',
  '转生',
  '科幻',
  '时间旅行',
  '性别转换',
  '萝莉',
  '传统游戏',
  '官方彩色',
  '历史',
  '怪物',
  '动作',
  '恶魔',
  '心理',
  '幽灵',
  '动物',
  '长条漫画',
  '恋爱',
  '忍者',
  '喜剧',
  '机甲',
  '选集',
  '耽美',
  '近亲',
  '犯罪',
  '生存',
  '僵尸',
  '逆后宫',
  '运动',
  '超级英雄',
  '武术',
  '粉丝上色',
  '武士',
  '魔法少女',
  '黑帮',
  '冒险',
  '自出版',
  '虚拟现实',
  '上班族',
  '电子游戏',
  '后启示录',
  '性暴力',
  '女装',
  '魔法',
  '百合',
  '后宫',
  '军事',
  '武侠',
  '异世界',
  '四格漫画',
  '同人志',
  '哲学',
  '血腥',
  '戏剧',
  '医疗',
  '校园生活',
  '恐怖',
  '奇幻',
  '恶役',
  '吸血鬼',
  '不良少年',
  '魔物娘',
  '正太',
  '警察',
  '网络漫画',
  '日常',
  '外星人',
  '料理',
  '超自然',
  '悬疑',
  '改编',
  '音乐',
  '全彩',
  '悲剧',
  '辣妹',
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
        <label class="filter-label">标题</label>
        <el-input v-model="filters.title" placeholder="请输入标题" class="filter-input" />
      </div>

      <div class="filter-item">
        <label class="filter-label">原始语言</label>
        <el-select
          v-model="filters.originalLanguage"
          multiple
          filterable
          clearable
          placeholder="任意"
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
        <label class="filter-label">翻译语言</label>
        <el-select
          v-model="filters.translatedLanguages"
          multiple
          filterable
          clearable
          placeholder="任意"
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
        <label class="filter-label">状态</label>
        <el-select
          v-model="filters.status"
          multiple
          filterable
          clearable
          placeholder="任意"
          class="filter-select"
        >
          <el-option v-for="item in statusOptions" :key="item" :label="item" :value="item" />
        </el-select>
      </div>

      <div class="filter-item">
        <label class="filter-label">内容评级</label>
        <el-select
          v-model="filters.contentRating"
          multiple
          filterable
          clearable
          placeholder="任意"
          class="filter-select"
        >
          <el-option v-for="item in contentRatingOptions" :key="item" :label="item" :value="item" />
        </el-select>
      </div>

      <div class="filter-item">
        <label class="filter-label">排序方式</label>
        <el-select
          v-model="filters.order"
          filterable
          clearable
          placeholder="无"
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
        <label class="filter-label">出版年份</label>
        <el-input v-model="filters.year" placeholder="任意" class="filter-input" />
      </div>

      <div class="filter-item filter-item-tags">
        <label class="filter-label"
          >包含标签
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
                >任意</span
              >
            </div>
          </template>
          <div class="tag-selection-content">
            <h4>已包含的标签</h4>
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
          >排除标签
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
                >任意</span
              >
            </div>
          </template>
          <div class="tag-selection-content">
            <h4>排除标签</h4>
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
        <el-button class="filter-search-btn" type="primary" @click="onSearchClick">搜索</el-button>
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
  min-height: 100vh;
  background: linear-gradient(135deg, #1a2035 0%, #2d3748 100%);
  position: relative;
  overflow: hidden;
}

.fliter-page::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url('data:image/svg+xml,<svg width="100" height="100" viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg"><rect width="100" height="100" fill="none"/><circle cx="50" cy="50" r="1" fill="rgba(255,255,255,0.1)"/></svg>')
    repeat;
  opacity: 0.1;
  pointer-events: none;
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
  transition:
    transform 0.3s ease,
    box-shadow 0.3s ease;
}

.manga-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.2);
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
  background: rgba(26, 32, 53, 0.8);
  backdrop-filter: blur(10px);
  padding: 25px;
  border-radius: 16px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.1);
  align-items: start;
  position: relative;
  z-index: 1;
}

.filter-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.filter-label {
  color: #e2e8f0;
  font-size: 0.95rem;
  font-weight: 500;
  margin-bottom: 0;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.filter-input,
.filter-select,
.filter-tag-trigger-text {
  width: 100%;
  height: 42px;
  background: rgba(255, 255, 255, 0.95);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 8px;
  color: #2d3748;
  font-size: 14px;
  box-sizing: border-box;
  padding: 0 12px;
  display: flex;
  align-items: center;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.filter-tag-trigger-text .placeholder-text {
  color: #a8abb2;
}

.filter-input:hover,
.filter-select:hover,
.filter-tag-trigger-text:hover {
  border-color: #4a5568;
  transform: translateY(-1px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.filter-input:focus-within,
.filter-select:focus-within,
.filter-tag-trigger-text:focus {
  border-color: #4299e1;
  box-shadow: 0 0 0 3px rgba(66, 153, 225, 0.2);
  outline: none;
}

.filter-item .filter-search-btn {
  width: 100%;
  height: 42px;
  margin-left: 0;
  background: linear-gradient(135deg, #4299e1 0%, #3182ce 100%);
  color: #fff;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  font-size: 14px;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(66, 153, 225, 0.2);
}

.filter-item .filter-search-btn:hover {
  background: linear-gradient(135deg, #3182ce 0%, #2c5282 100%);
  transform: translateY(-2px);
  box-shadow: 0 6px 12px rgba(66, 153, 225, 0.3);
}

.filter-item .filter-search-btn:active {
  transform: translateY(1px);
  box-shadow: 0 2px 4px rgba(66, 153, 225, 0.2);
}

.filter-item .filter-search-btn::after {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 5px;
  height: 5px;
  background: rgba(255, 255, 255, 0.5);
  opacity: 0;
  border-radius: 100%;
  transform: scale(1, 1) translate(-50%);
  transform-origin: 50% 50%;
}

.filter-item .filter-search-btn:active::after {
  animation: ripple 0.6s ease-out;
}

@keyframes ripple {
  0% {
    transform: scale(0, 0);
    opacity: 0.5;
  }
  100% {
    transform: scale(20, 20);
    opacity: 0;
  }
}

.tag-selection-content {
  background: #fff;
  border-radius: 12px;
  padding: 16px;
}

.tag-selection-content h4 {
  margin-top: 0;
  margin-bottom: 16px;
  color: #2d3748;
  font-size: 1.1rem;
  font-weight: 600;
}

.tag-list-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 10px;
}

.tag-item {
  cursor: pointer;
  transition: all 0.3s ease;
  border-radius: 6px;
  padding: 8px 12px;
}

.tag-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

:deep(.el-scrollbar__wrap) {
  margin-right: -10px !important;
}

:deep(.el-popover) {
  background-color: #fff !important;
  color: #2d3748 !important;
  border: none !important;
  border-radius: 12px !important;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.15) !important;
  padding: 0 !important;
}

:deep(.el-select-dropdown__item) {
  padding: 8px 12px;
  transition: all 0.3s ease;
}

:deep(.el-select-dropdown__item:hover) {
  background-color: #ebf8ff;
}

:deep(.el-select-dropdown__item.selected) {
  background-color: #4299e1;
  color: #fff;
}
</style>
