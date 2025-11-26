<script setup lang="ts">
definePageMeta({
  layout: 'auth'
})

const { $api } = useNuxtApp()

const isPM = ref(false)
const role = computed(() => isPM.value ? 'pm' : 'employee')

const form = ref({
  name: '',
  email: '',
  password: '',
})

const signup = async () => {
  try {
    await $api.post('/auth/register', {
      name: form.value.name,
      email: form.value.email,
      password: form.value.password,
      role: role.value
    })

    window.location.href = '/auth/signin'
  } catch (e) {
    console.error(e)
  }
}
</script>

<template>
    <div>
    <h1>Sign up</h1>

    <form @submit.prevent="signup">
      <label for="name">Name
        <input type="text" id="name" v-model="form.name" required />
      </label>
      
      <label for="email">Email
        <input type="email" id="email" v-model="form.email" required />
      </label>

      <label for="password">Password
        <input type="password" id="password" v-model="form.password" required />
      </label>

      <label for="role">
        I'm a PM
        <input type="checkbox" id="role" v-model="isPM">
      </label>

      <button type="submit">Sign in</button>
    </form>
  </div>
</template>