<script setup lang="ts">
const { $api } = useNuxtApp()

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
</script>

<template>
  <nav>
    <NuxtLink to="/">Home</NuxtLink> |
    <NuxtLink to="/projects">Projects</NuxtLink> |
    <NuxtLink to="/tasks">Tasks</NuxtLink>
    <button @click="logout">Logout</button>
  </nav>
</template>
