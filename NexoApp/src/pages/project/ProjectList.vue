<script lang="ts" setup>
import { onMounted, ref } from 'vue'

const projects = ref([])

onMounted(async () => {
  const jwt = localStorage.getItem('access_token')
  try {
    const data = await fetch('http://localhost:5015/projects', {
      headers: {
        Authorization: `Bearer ${jwt}`,
      },
    })

    projects.value = await data.json()
  } catch (e) {
    console.log(e)
  }
})
</script>

<template>
  <h1>Project list</h1>
  <pre>
    {{ projects }}
  </pre>
</template>
