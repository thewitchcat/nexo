<script setup lang="ts">
definePageMeta({
  layout: 'auth'
})

const { $api } = useNuxtApp()

const form = ref({
  email: '',
  password: '',
})

const signin = async () => {
  try {
    const { data } = await $api.post('/auth/login', {
      email: form.value.email,
      password: form.value.password
    })

    localStorage.setItem('access_token', data.accessToken)
    localStorage.setItem('refresh_token', data.refreshToken)

    window.location.href = '/'
  } catch (e) {
    console.error(e)
  }
}
</script>

<template>
  <div>
    <h1>Sign in</h1>

    <form @submit.prevent="signin">
      <label for="email">Email
        <input type="email" id="email" v-model="form.email" required />
      </label>

      <label for="password">Password
        <input type="password" id="password" v-model="form.password" required />
      </label>

      <button type="submit">Sign in</button>
    </form>
  </div>
</template>