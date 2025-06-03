//创建一个路由器，并暴露出去

//第一步，引入createRouter
import { createRouter, createWebHistory } from 'vue-router'
//引入要呈现的一个个组件
import Random from '@/views/Random.vue'
import Favorite from '@/views/Favorite.vue'
import Newest from '@/views/Newest.vue'
import Home from '@/views/Home.vue'
import Comic from '@/views/Comic.vue'
import Fliter from '@/views/Fliter.vue'
import ComicRead from '@/views/ComicRead.vue'
import ShowFilter from '@/views/ShowFilter.vue'
import ShowSearch from '@/views/ShowSearch.vue'
import component from 'element-plus/es/components/tree-select/src/tree-select-option.mjs'
//第二步，创建路由器
const routes = [
  {
    path: '/home',
    name: 'Home',
    component: Home,
  },
  { path: '/ShowSearch', component: ShowSearch },
  { path: '/newest', name: 'Newest', component: Newest },
  { path: '/favorite', name: 'Favorite', component: Favorite },
  { path: '/random', name: 'Random', component: Random },
  { path: '/comic/:id', name: 'Comic', component: Comic },
  {
    path: '/fliter',
    name: 'Fliter',
    component: Fliter,
    children: [
      {
        path: 'showfilter',
        component: ShowFilter,
      },
    ],
  }, // 如果你后面也想支持按id显示详情
  { path: '/read/:id', name: 'ComicRead', component: ComicRead },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
