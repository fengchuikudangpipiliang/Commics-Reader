// src/store/manga.ts
import { defineStore } from 'pinia'

export const useMangaStore = defineStore('manga', {
  state: () => ({
    lastManga: null as any,
    searchResultList: [] as any[],
    mangaFeedMap: {} as Record<string, any[]>,
  }),
  actions: {
    setMangaFeed(mangaId: string, feedList: any[]) {
      this.mangaFeedMap[mangaId] = feedList
    },
    setLastManga(manga: any) {
      this.lastManga = manga
    },
    setSearchResultList(list: any[]) {
      // 新增：设置搜索结果数组
      this.searchResultList = list
    },
    clearSearchResult(){
      this.searchResultList=[]
    }
  },
})
