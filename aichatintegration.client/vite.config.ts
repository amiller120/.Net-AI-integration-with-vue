import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue' // Or your framework plugin
import fs from 'fs'
import path from 'path'

// Get certificate path from environment or use default
const certPath = process.env.CERT_PATH || path.resolve(process.env.USERPROFILE || process.env.HOME, '.aspnet', 'https')

// Check if certificates exist
const keyPath = path.join(certPath, 'key.pem')
const certFilePath = path.join(certPath, 'cert.pem')
const useHttps = fs.existsSync(keyPath) && fs.existsSync(certFilePath)

console.log('VITE_API_BASE_URL:', process.env.VITE_API_BASE_URL)

export default defineConfig({
  plugins: [
    vue() // Replace with your framework plugin if needed
  ],
  server: {
    port: 7040,
    host: true,
    https: useHttps ? {
      key: fs.readFileSync(keyPath),
      cert: fs.readFileSync(certFilePath),
    } : undefined,
    // proxy: {
    //   '/api/chat': {
    //     target: 'https://aichatintegration.server:5001/chat',
    //     secure: false,
    //     changeOrigin: true
    //   }
    // }
  }
})
