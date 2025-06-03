
import { createApp } from 'vue'
import App from './App.vue'
import Router from './router'
// 引入 Element Plus
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
//引入pinia
import {createPinia} from 'pinia'
//引入persistedState
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import axios from "axios"
axios.defaults.baseURL = 'https://localhost:7274'

const app = createApp(App)
//创建pinia并使用
const pinia=createPinia()
pinia.use(piniaPluginPersistedstate)
app.use(pinia)
app.use(ElementPlus)
app.use(Router)
app.mount('#app')
