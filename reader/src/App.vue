<template>
  <el-container :class="{ 'landing-background': isLandingPage }">
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
        :placeholder="'点这里搜索...'"
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
          >搜索</el-button
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
    <!-- 看板娘聊天面板 -->
    <div v-if="showChatPanel" class="chat-panel">
      <div class="chat-header">
        <h3>看板娘对话</h3>
        <el-button link type="info" icon="CloseBold" @click="showChatPanel = false"></el-button>
      </div>
      <div class="chat-messages" ref="chatMessagesContainer">
        <div
          v-for="(message, index) in messages"
          :key="index"
          :class="['message-bubble', message.sender]"
        >
          {{ message.text }}
        </div>
      </div>
      <div class="chat-input-area">
        <el-input
          v-model="currentMessage"
          placeholder="和看板娘聊点什么..."
          @keyup.enter="sendMessage"
        ></el-input>
        <el-button type="primary" @click="sendMessage">发送</el-button>
      </div>
    </div>
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

// 新增聊天相关状态
const showChatPanel = ref(false)
const messages = ref<{ sender: 'user' | 'waifu'; text: string }[]>([])
const currentMessage = ref('')
const chatMessagesContainer = ref<HTMLElement | null>(null)

// 计算属性判断是否在landing page
const isLandingPage = computed(() => {
  return route.path === '/'
})

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
  //   // If logged in, show a confirmation or options (e.e., logout)
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

// 监听消息变化，滚动到底部
watch(
  messages,
  () => {
    nextTick(() => {
      if (chatMessagesContainer.value) {
        chatMessagesContainer.value.scrollTop = chatMessagesContainer.value.scrollHeight
      }
    })
  },
  { deep: true },
)

// 1️⃣ 先声明 say
function say(text: string, timeout = 6000) {
  messages.value.push({ sender: 'waifu', text: text }) // 将看板娘的回复添加到消息列表
}

// 2️⃣ 绑定按钮 & waifuSendMsg
onMounted(() => {
  window.addEventListener('load', () => {
    // 改变这里，点击按钮时切换我们自己的聊天面板
    document.querySelector('#waifu-tool-hitokoto')?.addEventListener('click', () => {
      showChatPanel.value = !showChatPanel.value
    })
  })
})

// 修改 waifuSendMsg 来处理消息发送和接收
;(window as any).waifuSendMsg = async () => {
  const msg = currentMessage.value.trim() // 从我们自己的输入框获取消息
  if (!msg) {
    say('你还没说话喵~')
    return
  }

  messages.value.push({ sender: 'user', text: msg }) // 将用户消息添加到列表
  currentMessage.value = '' // 清空输入框

  try {
    const { data } = await axios.get('/api/Deepseek/GetMsg', { params: { query: msg } })
    const reply: string = (data && (data.message ?? data)) || '喵？我没听清~'
    say(reply) // 调用修改后的 say 函数，将回复添加到列表
  } catch (err) {
    console.error(err)
    say('接口出错啦！')
  }
}

// 新增的发送消息函数，取代原始的 waifuSendMsg 逻辑
const sendMessage = async () => {
  await (window as any).waifuSendMsg()
}
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
  border: 2px solid #003366;
  border-radius: 10px;
  background: transparent;
  color: white;
  font-size: 16px;
  outline: none;
  padding: 8px 12px;
  transition: all 0.3s ease;
}

input:focus {
  border-color: #38bdf8;
  box-shadow: 0 0 8px rgba(56, 189, 248, 0.4);
  background: rgba(56, 189, 248, 0.05);
}

input:hover {
  border-color: #0ea5e9;
  background: rgba(14, 165, 233, 0.05);
}

input::placeholder {
  color: rgba(255, 255, 255, 0.6);
  transition: color 0.3s ease;
}

input:focus::placeholder {
  color: rgba(255, 255, 255, 0.8);
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
  /* text-align: center; */ /* 水平居中文字 */
  display: flex;
  justify-content: center;
  align-items: center;
}
.login-btn {
  background: #2563eb;
  color: #fff;
  border: none;
}
.login-btn:hover {
  background: #1a47a1;
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

/* 增强关闭按钮的可见性 */
:deep(.el-dialog__headerbtn) {
  position: absolute;
  top: 12px;
  right: 12px;
  padding: 8px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 50%;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  z-index: 10;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

:deep(.el-dialog__headerbtn:hover) {
  background: rgba(255, 255, 255, 0.2);
  transform: scale(1.1);
  border-color: rgba(255, 255, 255, 0.4);
}

:deep(.el-dialog__headerbtn .el-dialog__close) {
  color: #fff;
  font-size: 20px;
  font-weight: bold;
}

:deep(.el-dialog__headerbtn:hover .el-dialog__close) {
  color: #ff4d4f;
  transform: rotate(90deg);
}

/* 只在landing page应用背景 */
:deep(.el-container.landing-background) {
  background-image: url('/img/4.jpg');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}

/* Custom style for the warning message */
/* Targeting the specific type and custom class for higher specificity */
:deep(.el-message.el-message--warning.custom-warning-message) {
  background-color: rgba(25, 32, 53, 0.9) !important; /* Deep blue with transparency */
  color: #fff !important; /* White text */
  border-color: #1a2035 !important; /* Darker border */
  /* Adjust other styles as needed, e.g., padding */
}

:deep(.el-message.el-message--warning.custom-warning-message .el-message__icon) {
  color: #82b1ff !important; /* Example: Match filter button color */
}

:deep(.el-message.el-message--warning.custom-warning-message .el-message__closeBtn) {
  color: #fff !important; /* White close button */
}

/* 新增聊天面板样式 */
.chat-panel {
  position: fixed;
  bottom: 20px;
  right: 20px;
  width: 320px;
  height: 450px;
  background: rgba(45, 55, 72, 0.95); /* 深色半透明背景 */
  backdrop-filter: blur(8px);
  border-radius: 16px;
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.4);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  z-index: 1000;
  border: 1px solid rgba(255, 255, 255, 0.1);
  animation: fadeInRight 0.3s ease-out;
}

@keyframes fadeInRight {
  from {
    opacity: 0;
    transform: translateX(20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.chat-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 18px;
  background: rgba(26, 32, 53, 0.9);
  color: #e2e8f0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
  font-weight: 600;
}

.chat-header h3 {
  margin: 0;
  font-size: 1.1rem;
}

.chat-header .el-button {
  color: #e2e8f0;
  font-size: 18px;
  transition: color 0.3s ease;
}

.chat-header .el-button:hover {
  color: #ff4d4f; /* 红色关闭按钮 */
}

.chat-messages {
  flex: 1;
  padding: 15px;
  overflow-y: auto;
  -webkit-overflow-scrolling: touch;
  scroll-behavior: smooth;
}

.chat-messages::-webkit-scrollbar {
  width: 8px;
}

.chat-messages::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.1);
  border-radius: 10px;
}

.chat-messages::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.2);
  border-radius: 10px;
}

.message-bubble {
  max-width: 80%;
  padding: 10px 14px;
  border-radius: 18px;
  margin-bottom: 10px;
  line-height: 1.5;
  word-wrap: break-word;
  font-size: 0.95rem;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.15);
}

.message-bubble.user {
  background: linear-gradient(135deg, #4299e1 0%, #3182ce 100%);
  color: #fff;
  margin-left: auto;
  border-bottom-right-radius: 4px;
}

.message-bubble.waifu {
  background: rgba(255, 255, 255, 0.15);
  color: #e2e8f0;
  margin-right: auto;
  border-bottom-left-radius: 4px;
}

.chat-input-area {
  display: flex;
  padding: 15px;
  border-top: 1px solid rgba(255, 255, 255, 0.08);
  background: rgba(26, 32, 53, 0.9);
}

.chat-input-area .el-input {
  flex: 1;
  margin-right: 10px;
}

.chat-input-area .el-input :deep(.el-input__wrapper) {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  box-shadow: none !important;
  color: #e2e8f0;
  border: 1px solid rgba(255, 255, 255, 0.05);
}

.chat-input-area .el-input :deep(.el-input__inner) {
  color: #e2e8f0;
}

.chat-input-area .el-input :deep(.el-input__inner::placeholder) {
  color: rgba(255, 255, 255, 0.4);
}

.chat-input-area .el-button {
  background: linear-gradient(135deg, #4299e1 0%, #3182ce 100%);
  border: none;
  border-radius: 8px;
  font-weight: 600;
  transition: all 0.3s ease;
}

.chat-input-area .el-button:hover {
  background: linear-gradient(135deg, #3182ce 0%, #2c5282 100%);
  transform: translateY(-1px);
}
</style>
