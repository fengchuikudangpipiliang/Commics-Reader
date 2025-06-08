<template>
  <div class="reader-container" @scroll.passive="handleScroll" ref="scrollContainer">
    <!-- 主内容区 -->
    <div class="image-viewer">
      <div v-if="loading && !images.length" class="loading">Loading...</div>
      <!-- 加载完成后，如果没有图片，则显示暂无可读图片 -->
      <div v-else-if="!loading && !images.length" class="loading">暂无可读图片</div>
      <div v-else>
        <div v-for="(img, index) in visibleImages" :key="index" class="page-img">
          <!-- 前 3 张给 High，其余维持 Lazy -->
          <img
            :src="img"
            :fetchpriority="index < 3 ? 'high' : 'low'"
            :loading="index < 3 ? 'eager' : 'lazy'"
            alt="page image"
          />
        </div>
      </div>
    </div>
    <!-- 右侧悬浮按钮组 -->
    <div class="floating-sidebar">
      <!-- 返回按钮 -->
      <el-tooltip content="返回" placement="left">
        <button class="circle-btn" @click="goBack" style="font-size: medium">
          <i class="el-icon-arrow-left"></i>返回
        </button>
      </el-tooltip>
      <!-- 语言选择 -->
      <el-dropdown trigger="click" @command="onLanguageChangeDropdown">
        <button class="circle-btn" style="font-size: medium">
          <i class="el-icon-translate"></i>语言
        </button>
        <template #dropdown>
          <el-dropdown-menu>
            <!-- <el-dropdown-item
              v-for="lang in availableLanguages"
              :key="lang"
              :command="lang"
              :disabled="lang === selectedLanguage"
            >
              {{ lang.toUpperCase() }}
            </el-dropdown-item> -->
            <el-dropdown-item
              v-for="lang in availableLanguages"
              :key="lang"
              :command="lang"
              :disabled="lang === selectedLanguage"
            >
              {{ getChineseLanguageName(lang) }}（{{ chapterCountByLang[lang] }} 章）
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
      <!-- 章节选择 -->
      <el-dropdown trigger="click" @command="onChapterChangeDropdown">
        <button class="circle-btn" style="font-size: medium">
          <i class="el-icon-menu"></i>章节
        </button>
        <template #dropdown>
          <el-dropdown-menu>
            <div class="chapter-pagination">
              <el-button
                size="small"
                :disabled="offset === 0"
                @click="loadPreviousChapters"
                class="pagination-btn"
              >
                <i class="el-icon-arrow-left"></i>上一页
              </el-button>
              <!-- <span class="page-info"
                >{{ offset / limit + 1 }} / {{ Math.ceil(total / limit) }}</span
              > -->
              <el-button size="small" :disabled="!hasMore" @click="loadMore" class="pagination-btn">
                下一页<i class="el-icon-arrow-right"></i>
              </el-button>
            </div>
            <el-dropdown-item
              v-for="(ch, idx) in filteredChapters"
              :key="ch.id"
              :command="ch.id"
              :disabled="ch.id === selectedChapterId"
            >
              第{{ ch.displayIndex }}章（{{
                getChineseLanguageName(ch.attributes.translatedLanguage)
              }}）
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import { ref, onMounted, nextTick, computed } from 'vue'
import { ElMessage } from 'element-plus'

/* -------------------- 基础状态 -------------------- */
const route = useRoute()
const router = useRouter()
const mangaId = route.params.id as string

const images = ref<string[]>([]) // 全部图片
const visibleImages = ref<string[]>([]) // 已渲染图片
const scrollContainer = ref<HTMLElement | null>(null)
const BATCH_SIZE = 10 // 滚动一次追加多少

const chapterList = ref<any[]>([])
const selectedChapter = ref<any>(null)
const selectedLanguage = ref<string>('en')
const selectedChapterId = ref<string | null>(null)
const loading = ref(true)

/* -------------------- 工具 -------------------- */
const goBack = () => router.back()

const availableLanguages = computed(() => {
  const langs = chapterList.value.map((c) => c.attributes.translatedLanguage)
  return [...new Set(langs)]
})

const filteredChapters = computed(() => {
  const chapters = chapterList.value.filter(
    (ch) => ch.attributes.translatedLanguage === selectedLanguage.value,
  )
  return chapters.map((ch, idx) => ({
    ...ch,
    displayIndex: offset.value * limit + idx + 1,
  }))
})

const limit = 20
const offset = ref(0)
const total = ref(0)
//const chapterList = ref([])

const hasMore = computed(() => limit * (offset.value + 1) < total.value)

const chapterCountByLang = computed(() => {
  const countMap: Record<string, number> = {}
  chapterList.value.forEach((c) => {
    const lang = c.attributes.translatedLanguage
    countMap[lang] = (countMap[lang] || 0) + 1
  })
  return countMap
})

// Function to get Chinese language name from code
const getChineseLanguageName = (langCode: string) => {
  const languageMap: { [key: string]: string } = {
    en: '英语',
    zh: '中文',
    'zh-hk': '繁体中文',
    'pt-br': '葡萄牙语',
    es: '西班牙语',
    'es-la': '西班牙语',
    'ja-ro': '罗马字日语',
    'ko-ro': '罗马字韩语',
    'zh-ro': '罗马字中文',
    it: '意大利语',
    id: '印度尼西亚语 (印尼语)',
    tr: '土耳其语',
    vi: '越南语',
    de: '德语',
    fr: '法语',
    HI: '印地语',
    // Add more language mappings as needed
  }
  return languageMap[langCode.toLowerCase()] || langCode.toUpperCase()
}

const fetchChapters = async () => {
  try {
    loading.value = true

    const res = await axios.get('/api/Mangadex/GetChapterInfoById', {
      params: {
        uuid: mangaId,
        limit: limit,
        offset: offset.value,
      },
    })

    const data = res.data.data || []
    total.value = res.data.total || 0

    if (!data.length) {
      ElMessage.error('没有章节')
      return
    }

    // 替换章节列表，而不是追加
    chapterList.value = data

    // 可选：第一次加载时设置语言
    if (offset.value === 0) {
      const langCountMap: Record<string, number> = {}
      chapterList.value.forEach((c) => {
        const lang = c.attributes.translatedLanguage
        langCountMap[lang] = (langCountMap[lang] || 0) + 1
      })

      const sorted = Object.entries(langCountMap).sort((a, b) => b[1] - a[1])
      selectedLanguage.value = sorted[0]?.[0] || 'en'

      // 确保在设置语言后立即开始加载图片
      await nextTick()
      onLanguageChange()
    }
  } catch (e) {
    ElMessage.error('获取章节失败')
  } finally {
    loading.value = false
  }
}

// 加载更多章节（分页）
const loadMore = () => {
  if (hasMore.value) {
    offset.value++
    fetchChapters()
  }
}

const loadPreviousChapters = () => {
  if (offset.value > 0) {
    offset.value--
    fetchChapters()
  }
}

/* -------------------- 语言切换 -------------------- */
const onLanguageChange = async () => {
  images.value = []
  visibleImages.value = []
  loading.value = true

  const chapters = chapterList.value.filter(
    (ch) => ch.attributes.translatedLanguage === selectedLanguage.value,
  )
  if (!chapters.length) {
    ElMessage.warning('该语言下暂无可读章节')
    loading.value = false
    return
  }
  selectedChapter.value = chapters[0]
  selectedChapterId.value = selectedChapter.value.id

  // 确保状态更新后再开始获取图片
  await nextTick()
  fetchChapterImages()
}

const onChapterChange = () => {
  selectedChapter.value = chapterList.value.find((ch) => ch.id === selectedChapterId.value)
  if (!selectedChapter.value) {
    ElMessage.warning('未找到该章节')
    return
  }
  images.value = []
  visibleImages.value = []
  loading.value = true
  fetchChapterImages()
}

/* -------------------- SSE 获取图片（队列 + 调度器） -------------------- */
const decodeQueue: string[] = [] // 待渲染队列
let eventSource: EventSource | null = null
let isClosed = false // 防止重复关闭

// 每帧把服务端推送回来的 URL 放进 images 池；不直接渲染
const pump = () => {
  if (decodeQueue.length) {
    images.value.push(decodeQueue.shift()!)
    // 如果首屏尚未渲染满 BATCH_SIZE，则立即补足
    if (visibleImages.value.length < BATCH_SIZE) {
      const next = images.value.slice(visibleImages.value.length, BATCH_SIZE)
      visibleImages.value.push(...next)
    }
  }
  requestAnimationFrame(pump)
}

const fetchChapterImages = () => {
  if (!selectedChapter.value) return
  if (eventSource) eventSource.close()

  isClosed = false
  loading.value = true
  images.value = []
  visibleImages.value = []

  eventSource = new EventSource(
    `https://localhost:7274/api/Mangadex/GetChapterImgStream?chapterId=${selectedChapter.value.id}`,
  )

  eventSource.onmessage = (e) => {
    const data = e.data

    if (data === 'complete') {
      if (!isClosed) {
        eventSource!.close()
        isClosed = true
        loading.value = false
      }
      return
    }
    // 推入待渲染队列
    decodeQueue.push(data)
  }

  eventSource.onerror = (err) => {
    if (!isClosed) {
      loading.value = false
      eventSource!.close()
      isClosed = true
    }
  }
}

/* -------------------- 滚动追加 -------------------- */
const handleScroll = () => {
  if (!scrollContainer.value || loading.value) return
  const el = scrollContainer.value
  const nearBottom = el.scrollTop + el.clientHeight >= el.scrollHeight - 100 // 留100px阈值

  if (nearBottom) {
    // 还没显示完当前章节所有图片就追加
    const nextBatch = images.value.slice(
      visibleImages.value.length,
      visibleImages.value.length + BATCH_SIZE,
    )
    if (nextBatch.length) {
      visibleImages.value.push(...nextBatch)
    } else {
      // 当前章节所有图片都已显示
      // 检查是否有下一章节
      const currentChapterIndex = filteredChapters.value.findIndex(
        (ch) => ch.id === selectedChapterId.value,
      )
      const nextChapterIndex = currentChapterIndex + 1

      if (nextChapterIndex < filteredChapters.value.length) {
        // 有下一章节，加载下一章节
        const nextChapter = filteredChapters.value[nextChapterIndex]
        selectedChapterId.value = nextChapter.id
        onChapterChange() // 调用加载章节的函数
        ElMessage.info(`正在加载下一章节: 第${nextChapter.displayIndex}章`)
      } else {
        // 没有下一章节了
        ElMessage.info('已经是最后一章节了')
      }
    }
  }
}

/* -------------------- 下拉菜单事件 -------------------- */
const onLanguageChangeDropdown = (lang: string) => {
  if (lang !== selectedLanguage.value) {
    selectedLanguage.value = lang
    onLanguageChange()
  }
}

const onChapterChangeDropdown = (chapterId: string) => {
  if (chapterId !== selectedChapterId.value) {
    selectedChapterId.value = chapterId
    onChapterChange()
  }
}

/* -------------------- 启动 -------------------- */
onMounted(async () => {
  await fetchChapters()
  await nextTick()
  scrollContainer.value = document.querySelector('.reader-container')
  pump() // 启动渲染调度器
})
</script>

<style scoped>
.reader-container {
  background: #0f1624;
  min-height: 100vh;
  max-height: 100vh;
  overflow-y: auto;
  display: flex;
  flex-direction: row;
  align-items: flex-start;
  position: relative;
}
.image-viewer {
  flex: 1;
  max-width: 900px;
  margin: 0 auto;
  padding: 0 20px;
}
.floating-sidebar {
  position: fixed;
  top: 50%;
  right: 32px;
  transform: translateY(-50%);
  display: flex;
  flex-direction: column;
  gap: 18px;
  z-index: 30;
}
.circle-btn {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  background: linear-gradient(135deg, #1e293b 80%, #334155 100%);
  color: #fff;
  border: none;
  box-shadow: 0 2px 12px rgba(30, 41, 59, 0.18);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 22px;
  cursor: pointer;
  transition:
    box-shadow 0.2s,
    transform 0.2s,
    background 0.2s;
}
.circle-btn:hover {
  box-shadow:
    0 4px 24px #38bdf8aa,
    0 2px 12px rgba(30, 41, 59, 0.18);
  background: linear-gradient(135deg, #38bdf8 80%, #0ea5e9 100%);
  color: #fff;
  transform: scale(1.08) translateY(-2px);
}
.page-img {
  margin-bottom: 1rem;
  display: flex;
  justify-content: center;
}
.page-img img {
  width: 100%;
  border-radius: 4px;
  box-shadow: 0 0 12px rgba(255, 255, 255, 0.1);
}
.loading {
  color: #fff;
  font-size: 1.2rem;
  text-align: center;
  margin-top: 100px;
}
.chapter-pagination {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 12px;
  border-bottom: 1px solid #dcdfe6;
  margin-bottom: 8px;
}

.pagination-btn {
  padding: 4px 8px;
  font-size: 12px;
}

.page-info {
  font-size: 12px;
  color: #606266;
}

:deep(.el-dropdown-menu) {
  min-width: 200px;
}
</style>
