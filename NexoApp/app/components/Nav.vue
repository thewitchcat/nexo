<script setup lang="ts">
type User = {
  id: number
  name: string
  email: string
  role: string
}

const { $api } = useNuxtApp()

const currentLoggedInUser = ref<User | null>(null)
const loadCurentLoggedInUser = async () => {
  try {
    const { data } = await $api.get('/auth/me')
    currentLoggedInUser.value = data
  } catch (e) {
    console.error(e)
  }
}

const logout = async () => {
  try {
    await $api.post('/auth/logout')

    localStorage.removeItem('access_token')
    localStorage.removeItem('refresh_token')
    window.location.href = '/auth/signin'
  } catch (e) {
    console.error(e)
  }
}

loadCurentLoggedInUser()
</script>

<template>
  <nav>
    <p v-if="currentLoggedInUser == null">Loading...</p>
    <p v-else>Hello {{ currentLoggedInUser?.name }}</p>
    <NuxtLink to="/">Home</NuxtLink> |
    <NuxtLink to="/projects">Projects</NuxtLink> |
    <NuxtLink to="/tasks">Tasks</NuxtLink>
    <button @click="logout">Logout</button>
  </nav>
</template>
