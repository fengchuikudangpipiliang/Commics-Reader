<!-- src/views/Newest.vue -->
<template>
  <div class="loading-container">
    <el-skeleton rows="3" animated />
    <p>正在为您推荐一部随机漫画，请稍候...</p>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { ElMessage } from 'element-plus'

const router = useRouter()

onMounted(async () => {
  try {
    const res = await axios.get('/api/Mangadex/GetRandomManga')

    // 提取 UUID（即漫画 ID）
    const uuid = res.data?.data?.[0]?.id

    if (uuid && typeof uuid === 'string') {
      // 自动跳转到 Comic.vue
      router.replace({ name: 'Comic', params: { id: uuid } })
    } else {
      ElMessage.error('未获取到有效的漫画 ID')
    }
  } catch (err) {
    console.error('获取随机漫画失败:', err)
    ElMessage.error('加载失败，请稍后重试')
  }
})
</script>

<style scoped>
.loading-container {
  max-width: 600px;
  margin: 100px auto;
  text-align: center;
  color: #ccc;
  font-size: 1.2rem;
}
</style>
