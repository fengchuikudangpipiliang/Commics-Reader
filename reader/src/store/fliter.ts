//存进pinia
import { defineStore } from 'pinia'
import type { Filter } from '@/type/filter'
export const useFilterStore = defineStore('filter', {
  state: (): { filter: Filter } => ({
    filter: {
      title: '',
      originalLanguage: [],
      translatedLanguages: [],
      status: [],
      contentRating: [],
      year: '',
      limit: 20,
      offset: 0,
      order: '',
      includedTags: [],
      excludedTags: [],
    },
  }),
  actions: {
    //存储用户信息
    setFilter(filter: Filter) {
      this.filter = filter
    },
  },
  persist: true, //安装了pinia-plugin-persistedstate
})
