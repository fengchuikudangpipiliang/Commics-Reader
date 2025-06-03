export interface Filter {
  title: string
  originalLanguage: string[]
  translatedLanguages: string[]
  status: string[]
  contentRating: string[]
  year: string
  limit: number
  offset: number
  order: string
  includedTags: string[]
  excludedTags: string[]
}
