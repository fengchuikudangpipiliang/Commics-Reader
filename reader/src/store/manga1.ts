import { defineStore } from 'pinia'
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

export const useManga1Store = defineStore('manga1', {
  state: () => ({
    mangaSearchResult: null as MangaData1[] | null,
  }),
  actions: {
    setMangaSearchResult(result: MangaData1[]) {
      this.mangaSearchResult = result
    },
  },
})
