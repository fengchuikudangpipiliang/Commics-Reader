import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

export default defineConfig({
  plugins: [vue(), vueDevTools()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    host: true, // ✅ 监听 0.0.0.0，确保外部（如 ngrok）可以访问
    allowedHosts: ['.ngrok-free.app'], // ✅ 允许 ngrok 的 host 访问

    proxy: {
      '/api': {
        target: 'http://localhost:7274',
        changeOrigin: true,
        secure: false, // ✅ 自签名证书需设为 false
        rewrite: (path) => path.replace(/^\/api/, ''),
      },
    },
  },
})
