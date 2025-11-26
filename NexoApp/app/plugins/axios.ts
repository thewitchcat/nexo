import axios from 'axios'

export default defineNuxtPlugin((nuxtApp) => {
  const api = axios.create({
    baseURL: 'http://localhost:5015'
  })

  api.interceptors.request.use((config) => {
    const accessToken = localStorage.getItem('access_token')
    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`
    }

    return config
  })

  api.interceptors.response.use(
    (response) => response,
    async (e) => {
      if (e.response?.status === 401) {
        try {
          const refreshToken = localStorage.getItem('refresh_token')
          const { data } = await api.post('/auth/refresh-token', {
            refreshToken
          })

          localStorage.setItem('access_token', data.accessToken)
          localStorage.setItem('refresh_token', data.refreshToken)

          e.config.headers.Authorization = `Bearer ${data.accessToken}`

          return api(e.config)
        } catch (e) {
          localStorage.removeItem('access_token')
          localStorage.removeItem('refresh_token')
          window.location.href = '/signin'
        }
      }

      return Promise.reject(e)
    }
  )

  return {
    provide: {
      api
    }
  }
})