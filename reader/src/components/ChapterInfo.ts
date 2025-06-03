// Title
export interface Title {
  en: string
  [key: string]: string // 兼容其它语言如 ja/zh 等
}

// CoverArtAttributes
export interface CoverArtAttributes1 {
  description: string
  volume: string
  fileName: string
  locale: string
  createdAt: string // C# DateTime 映射 string
  updatedAt: string
  version: number
}

// MangaRelationship
export interface MangaRelationship {
  id: string
  type: string
  related?: string
  attributes?: CoverArtAttributes1
}

// TagAttributes
export interface TagAttributes {
  name: { [key: string]: string }
  description: { [key: string]: string }
  group: string
  version: number
}

// MangaTag
export interface MangaTag {
  id: string
  type: string
  attributes: TagAttributes
}

// MangaAttributes
export interface MangaAttributes {
  title: Title
  altTitles: Array<{ [key: string]: string }>
  description: { [key: string]: string }
  isLocked: boolean
  links?: { [key: string]: string }
  originalLanguage: string
  lastVolume: string
  lastChapter: string
  publicationDemographic?: string
  status: string
  year: number
  contentRating: string
  tags: MangaTag[]
  state: string
  chapterNumbersResetOnNewVolume: boolean
  createdAt: string
  updatedAt: string
  version: number
  availableTranslatedLanguages: string[]
  latestUploadedChapter: string
}

// MangaData1
export interface MangaData1 {
  id: string
  type: string
  attributes: MangaAttributes
  relationships: MangaRelationship[]
}

// MangaSearchTitleResponse
export interface MangaSearchTitleResponse {
  result: string
  response: string
  data: MangaData1[]
  limit: number
  offset: number
  total: number
}
