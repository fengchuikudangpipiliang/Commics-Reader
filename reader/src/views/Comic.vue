<template>
  <div class="comic-detail-container">
    <div v-if="loading" class="loading">
      <el-skeleton :rows="10" animated />
    </div>
    <div v-else-if="!comic">
      <p style="color: #ff8585; font-size: 1.4rem; margin: 40px 0">
        未找到该漫画，请返回首页或检查漫画ID
      </p>
    </div>
    <div v-else>
      <div class="comic-header">
        <div class="cover-box">
          <img :src="comic.cover" class="cover-img" :alt="comic.title" />
        </div>
        <div class="main-info">
          <div class="status">{{ comic.status }}</div>
          <h1 class="title">{{ comic.title }}</h1>
          <div class="btn-row">
            <el-button type="primary" size="large" @click="StartReading">START READING</el-button>
            <el-button size="large" plain @click="handleFavoriteClick">
              {{ isFavorited ? '取消收藏' : '收藏' }}
              <el-icon><i class="el-icon-collection"></i></el-icon>
            </el-button>
          </div>
          <div class="desc">
            {{ comic.desc }}
          </div>
          <div class="detail-list">
            <div><b>Author:</b> {{ comic.author }}</div>
            <div><b>Published:</b> {{ comic.published }}</div>
            <div>
              <b>Genres:</b>
              <span v-for="g in comic.genres" :key="g" class="genre-tag">{{ g }}</span>
            </div>
          </div>
          <div class="rating-box">
            <span class="rating">{{ rating.toFixed(1) }}</span>
            <span class="rating-sub">/ 10</span>
            <el-rate :model-value="displayScore" :max="5" disabled />
            <span class="raters">by {{ raters }} reviews</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import { useUserStore } from '@/store/login'
import { ElMessage, ElMessageBox } from 'element-plus'

const userStore = useUserStore()
const isFavorited = ref(false) //当前是否收藏
const loading = ref(true) // 添加loading状态

const router = useRouter()
const route = useRoute()
const comicId = ref(route.params.id as string) // 获取路由中的 comicId

// 新增评分相关响应式变量
const rating = ref(0)
const raters = ref(0)

// 处理开始阅读按钮点击
const StartReading = () => {
  router.push(`/read/${comicId.value}`)
}

//处理收藏/取消收藏
const handleFavoriteClick = async () => {
  if (!userStore.userId) {
    ElMessageBox.alert('请先登录以收藏漫画', '提示', { type: 'warning' })
    return
  }

  const userId = userStore.userId
  const mangaId = comic.value?.id

  if (!mangaId) {
    ElMessage.error('漫画信息有误')
    return
  }

  try {
    if (!isFavorited.value) {
      await axios.post('/api/UsersLove/AddUsersLove', null, {
        params: {
          uuid: mangaId,
          id: userId,
        },
      })
      console.log('收藏')
      isFavorited.value = true
      ElMessage.success('已收藏')
    } else {
      await axios.post('/api/UsersLove/SubUsersLove', null, {
        params: {
          uuid: mangaId,
          id: userId,
        },
      })
      console.log('取消收藏')

      isFavorited.value = false
      ElMessage.success('已取消收藏')
    }
  } catch (err) {
    ElMessage.error('操作失败，请稍后重试')
    console.error(err)
  }
}

// 获取漫画详情
const comic = ref<any>(null)

onMounted(async () => {
  try {
    loading.value = true // 开始加载
    const response = await axios.get(
      `https://api.mangadex.org/manga/${comicId.value}?includes[]=author&includes[]=artist&includes[]=cover_art`,
    )
    const data = response.data.data

    if (data) {
      const coverRel = data.relationships?.find((r: any) => r.type === 'cover_art')
      const fileName = coverRel?.attributes?.fileName || ''
      const cover = fileName ? `https://uploads.mangadex.org/covers/${data.id}/${fileName}` : ''
      const title = data.attributes?.title?.en || '未知标题'
      const status = data.attributes?.status || 'Unknown'
      const desc = data.attributes?.description?.en || ''
      const author =
        data.relationships?.find((r: any) => r.type === 'author')?.attributes?.name || '未知'
      const published = data.attributes?.year || ''

      const genres = (data.attributes?.tags || [])
        .map((tag: any) => tag.attributes?.name?.en)
        .filter(Boolean)

      comic.value = {
        id: data.id,
        cover,
        status,
        title,
        desc,
        author,
        published,
        genres,
      }

      // 获取评分信息
      await fetchRating(data.id)
    } else {
      console.error('未找到漫画数据:', comicId.value)
    }
  } catch (error) {
    console.error('获取漫画详情失败:', error)
  } finally {
    loading.value = false // 结束加载
  }
})

// 获取漫画评分
async function fetchRating(comicId: string) {
  try {
    const response = await axios.get(`/api/Mangadex/GetCommetnById`, {
      params: { uuid: comicId },
    })

    const comicStats = response.data.statistics?.[comicId]
    if (comicStats) {
      rating.value = comicStats.rating?.average || 0
      raters.value = comicStats.follows || 0
    } else {
      console.warn('未找到该漫画的评分数据:', comicId)
    }
  } catch (error) {
    console.error('获取评分失败:', error)
    rating.value = 0
    raters.value = 0
  }
}

// 显示评分（转换为5分制）
const displayScore = computed(() => rating.value / 2)
</script>

<style scoped>
/* 添加loading样式 */
.loading {
  max-width: 1320px;
  margin: 38px auto 0 auto;
  padding: 0 18px;
}

/* 样式保持不变 */
.comic-detail-container {
  max-width: 1320px;
  margin: 38px auto 0 auto;
  padding: 0 18px;
  color: #e2e6f3;
}
.comic-header {
  display: flex;
  gap: 40px;
  background: #222939;
  border-radius: 22px;
  padding: 38px 36px 38px 36px;
  box-shadow: 0 2px 32px rgba(0, 0, 0, 0.14);
}
.cover-box {
  min-width: 215px;
  max-width: 215px;
}
.cover-img {
  width: 215px;
  height: 300px;
  border-radius: 12px;
  object-fit: cover;
  box-shadow: 0 2px 16px rgba(0, 0, 0, 0.18);
  background: #232b37;
}
.main-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.status {
  color: #b5bcd3;
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 0;
  letter-spacing: 1.2px;
}
.title {
  font-size: 2.1rem;
  color: #fff;
  font-weight: 700;
  margin: 2px 0 7px 0;
  letter-spacing: 1.2px;
}
.btn-row {
  margin: 10px 0 14px 0;
  display: flex;
  gap: 18px;
  align-items: center;
}
.meta-row {
  color: #b7bdd6;
  margin-bottom: 12px;
  display: flex;
  gap: 22px;
  font-size: 1.06rem;
  font-weight: 500;
}
.desc {
  color: #c6cbdb;
  margin-bottom: 13px;
  font-size: 1.05rem;
}
.detail-list {
  font-size: 1.06rem;
  color: #d6daea;
  margin-bottom: 8px;
}
.genre-tag {
  display: inline-block;
  background: #283652;
  color: #ffd04b;
  font-size: 0.98rem;
  border-radius: 11px;
  padding: 2px 11px;
  margin-right: 8px;
}
.rating-box {
  margin: 13px 0 0 0;
  display: flex;
  align-items: center;
  gap: 8px;
}
.rating {
  font-size: 2.1rem;
  font-weight: bold;
  color: #ffd04b;
}
.rating-sub {
  font-size: 1.3rem;
  color: #ffd04b;
}
.raters {
  font-size: 0.98rem;
  color: #c0c6df;
  margin-left: 12px;
}
</style>
