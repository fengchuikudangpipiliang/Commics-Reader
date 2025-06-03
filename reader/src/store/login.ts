
//将用户id存进pinia
import { defineStore } from 'pinia'
export const useUserStore = defineStore('user', {
  state: () => ({
    userId: null as number | null,
  }),
  actions: {
    //存储用户信息
    setUserInfo(userId: number) {
      this.userId = userId
    },
    //删除用户信息
    clearUserInfo() {
      this.userId = null
    },
  },
  persist: true, //安装了pinia-plugin-persistedstate
})
