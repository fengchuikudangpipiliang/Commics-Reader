<template>
  <el-container>
    <!-- 顶部导航栏 -->
    <el-header class="nav-bar">
      <div class="nav-logo">
        <!-- 建议把 src="#" 改成你自己的 logo 地址，或者注释掉这行 -->
        <img src="@/resource/imgs/11.jpg" alt="MangaFire Logo" class="logo-img" />
        <span class="site-title">gulugulu<span class="bold"></span><span class="ver"></span></span>
      </div>
      <div class="nav-menu">
        <el-menu
          mode="horizontal"
          :default-active="activeMenu"
          class="el-menu-demo"
          background-color="transparent"
          text-color="#fff"
          active-text-color="#ffd04b"
          router
        >
          <el-menu-item index="/home">首页</el-menu-item>
          <el-menu-item index="/newest">最新</el-menu-item>
          <el-menu-item index="/favorite">收藏</el-menu-item>
          <el-menu-item index="/random">随机</el-menu-item>
          <el-menu-item index="/fliter">高级搜索</el-menu-item>
        </el-menu>
      </div>
      <input
        type="text"
        v-model="searchTitle"
        :placeholder="'click here to search ...'"
        :disabled="isFilterPage"
        style="margin-right: 10px; width: 220px"
      />
      <div class="nav-action">
        <el-button
          class="filter-btn"
          type="primary"
          plain
          icon="Filter"
          @click="handleFilterClick"
          :disabled="isFilterPage"
          >Filter</el-button
        >
        <el-button class="login-btn" type="primary" @click="handleLoginButtonClick">
          {{ isLoggedIn ? '已登录' : '登录/注册' }}
        </el-button>
      </div>
    </el-header>
    <!-- 路由页面内容 -->
    <el-main style="padding: 0; margin: 0 50px">
      <RouterView :key="$route.fullPath"></RouterView>
    </el-main>
    <!-- 登录弹窗 -->
    <el-dialog
      v-model="showLogin"
      width="480px"
      :close-on-click-modal="false"
      :show-close="true"
      center
      @closed="closeLoginDialog"
    >
      <Login :is-logged-in="isLoggedIn" @close-dialog="closeLoginDialog" />
    </el-dialog>
  </el-container>
</template>

<script setup lang="ts">
import { ref, toRefs, computed, watch, nextTick, onMounted } from 'vue'
import axios, { type AxiosResponse } from 'axios'
import { useRouter, useRoute } from 'vue-router'
import Login from './page/Login.vue'
import { useManga1Store } from '@/store/manga1' // 导入 store
import { useUserStore } from '@/store/login' // Import the user store
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
import { ElMessage, ElMessageBox } from 'element-plus'

const showLogin = ref(false)
const searchTitle = ref('') // 输入框内容
const loading = ref(false)
const router = useRouter()
const route = useRoute()
const activeMenu = ref('')
const mangaStore = useManga1Store() // 使用 store
const userStore = useUserStore() // Use the user store

// 计算属性判断是否在高级搜索页面
const isFilterPage = computed(() => {
  return route.path.startsWith('/fliter')
})

// Computed property to check if the user is logged in
const isLoggedIn = computed(() => {
  // Assuming userStore.userId exists and is non-null when logged in
  return !!userStore.userId
})

// Handle click on Login/Register button
const handleLoginButtonClick = () => {
  // Always show the login dialog for now
  showLogin.value = true

  // The logic for handling logged-in state click will be inside Login.vue
  // if (isLoggedIn.value) {
  //   // If logged in, show a confirmation or options (e.g., logout)
  //   // We will handle this inside the Login component
  // } else {
  //   // If not logged in, show the login dialog
  //   showLogin.value = true;
  // }
}

// Function to close the login dialog (will be passed to Login.vue)
const closeLoginDialog = () => {
  showLogin.value = false
}

// 处理搜索并跳转或提示
async function handleFilterClick() {
  if (searchTitle.value.trim()) {
    // 如果搜索框有内容，执行搜索并跳转到搜索结果页
    router.push({
      path: '/ShowSearch',
      query: {
        title: searchTitle.value,
      },
    })
  } else {
    // 输入框为空
    ElMessage({
      message: '您还没有输入内容',
      type: 'warning',
      duration: 2000,
      customClass: 'custom-warning-message',
    })
  }
}

// 路由变化就跳转
watch(
  () => route.path,
  (newPath) => {
    if (newPath.startsWith('/fliter')) {
      searchTitle.value = ''
    }
    // Keep active menu updated with route change
    activeMenu.value = newPath
  },
  { immediate: true }, // Run on initial mount
)

// Set active menu on initial load
onMounted(() => {
  activeMenu.value = route.path
})
</script>

<style scoped>
/* 自定义覆盖 ElMessage 样式 */
.el-message {
  background-color: rgba(17, 85, 204, 0.8); /* 深蓝色半透明 */
  color: #fff; /* 文字白色 */
  border-color: rgba(17, 85, 204, 0.9); /* 边框色 */
}

.el-message--warning {
  background-color: rgba(17, 85, 204, 0.8);
  color: #fff;
  border-color: rgba(17, 85, 204, 0.9);
}

.nav-bar {
  background: #131629;
  color: #fff;
  display: flex;
  align-items: center;
  height: 60px;
  padding: 0 36px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}
.nav-logo {
  display: flex;
  align-items: center;
  margin-right: 36px;
}
.logo-img {
  height: 38px;
  margin-right: 8px;
}
.site-title {
  font-size: 22px;
  font-weight: 600;
  color: #fff;
}
.site-title .bold {
  font-weight: bold;
  color: #ffd04b;
}
.site-title .ver {
  font-size: 16px;
  font-weight: 400;
  color: #82b1ff;
}
.nav-menu {
  flex: 1;
  min-width: 480px;
}
.el-menu-demo {
  background: transparent;
  border-bottom: none;
}
input {
  flex: 1;
  border: 2px solid #003366; /* 添加深蓝色边框，宽度为2px */
  border-radius: 10px; /* 设置圆角，数值可按需调整 */
  background: transparent;
  color: white;
  font-size: 16px;
  outline: none;
}
.nav-action {
  display: flex;
  align-items: center;
  gap: 18px;
}
.filter-btn {
  margin-left: 30px;
  background: #212a45;
  color: #82b1ff;
  border: none;
  text-align: center; /* 水平居中文字 */
}
.login-btn {
  background: #2563eb;
  color: #fff;
  border: none;
}
.login-btn:hover {
  background: #1a47a1;
}

/* Custom style for the warning message */
/* Targeting the specific type and custom class for higher specificity */
:deep(.el-message.el-message--warning.custom-warning-message) {
  background-color: rgba(25, 32, 53, 0.9) !important; /* Deep blue with transparency */
  color: #fff !important; /* White text */
  border-color: #1a2035 !important; /* Darker border */
  /* Adjust other styles as needed, e.g., padding */
}

/* You might need to override default icon/close button colors if they don't match */
:deep(.el-message.el-message--warning.custom-warning-message .el-message__icon) {
  color: #82b1ff !important; /* Example: Match filter button color */
}

:deep(.el-message.el-message--warning.custom-warning-message .el-message__closeBtn) {
  color: #fff !important; /* White close button */
}

/* Style for the ElDialog itself to remove the white frame */
:deep(.el-dialog) {
  background-color: transparent !important; /* Make the dialog background transparent */
  box-shadow: none !important; /* Remove default box shadow */
  border: none !important; /* Ensure no border */
}

/* Ensure no padding on dialog body if necessary */
:deep(.el-dialog__body) {
  padding: 0 !important; /* Remove default body padding */
}

/* Ensure no padding on dialog header/footer if they exist and have padding */
:deep(.el-dialog__header),
:deep(.el-dialog__footer) {
  padding: 0 !important;
}
</style>
