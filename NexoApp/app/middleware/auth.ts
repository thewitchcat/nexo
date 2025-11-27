export default defineNuxtRouteMiddleware((to, from) => {
  if (typeof window === 'undefined') return
  
  const token = localStorage.getItem('access_token')

  if (!token) {
    window.location.href = '/auth/signin'
  }
})