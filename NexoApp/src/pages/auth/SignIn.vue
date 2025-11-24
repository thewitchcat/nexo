<script lang="ts" setup>
import { ref } from 'vue'

const form = ref({
  email: '',
  password: '',
})

async function signin() {
  try {
    const data = await fetch('http://localhost:5015/auth/login', {
      method: 'POST',
      body: JSON.stringify(form.value),
      headers: {
        'Content-Type': 'application/json',
      },
    })

    const jwt = await data.json()
    localStorage.setItem('access_token', jwt.accessToken)
    localStorage.setItem('refresh_token', jwt.refreshToken)
  } catch (e) {
    console.log(e)
  }
}
</script>

<template>
  <div class="container">
    <el-card class="card">
      <h1 class="title">Sign in</h1>
      <p class="subtitle">to continue to Nexo</p>

      <form @submit.prevent="signin">
        <span>Email</span>
        <el-input v-model="form.email" placeholder="kellnergohn@galaxy.com" />

        <span>Password</span>
        <el-input
          v-model="form.password"
          placeholder="password-accross-galaxies"
          type="password"
          show-password
        />
      </form>
      <el-button @click="signin">Sign in</el-button>
    </el-card>
  </div>
</template>

<style scoped>
.title {
  font-size: var(--el-font-size-large);
  margin: 0px;
  font-weight: 600;
}

.subtitle {
  font-size: var(--el-font-size-medium);
  margin: 0px;
}

.card {
  max-width: 20vw;
  margin-left: auto;
  margin-right: auto;
  width: 100%;
}
</style>
