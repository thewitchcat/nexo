<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

type User = {
  id: number
  name: string
  email: string
  role: string
}

type Project = {
  id: number
  name: string
  description: string
}

type ProjectForm = Omit<Project, 'id'>

const { $api } = useNuxtApp()

const router = useRouter()
const route = useRoute()

const currentLoggedInUser = ref<User | null>(null)
const loadCurentLoggedInUser = async () => {
  try {
    const { data } = await $api.get('/auth/me')
    currentLoggedInUser.value = data
  } catch (e) {
    console.error(e)
  }
}

const userId = ref(0)
const isNew = route.params.projectId === 'new'
const form = ref<ProjectForm>({
  name: '',
  description: '',
})

const submitForm = async () => {
  try {
    isNew ? await $api.post('/projects', { 
      name: form.value.name,
      description: form.value.description,
      createdBy: userId.value,
    }) : await $api.put(`/projects/${route.params.projectId}`, {
      name: form.value.name,
      description: form.value.description,
      createdBy: userId.value,
    })

    form.value = {
      name: '',
      description: '',
    }

    userId.value = 0

    router.push('/projects')
  } catch (e) {
    console.error(e)
  }
}

if (!isNew) {
  const { data } = await $api.get(`/projects/${route.params.projectId}`)

  if (data) {
    form.value = {
      name: data.name ?? '',
      description: data.description ?? '',
    }

    userId.value = data.createdBy ?? 0
  }
}


loadCurentLoggedInUser();
onMounted(() => {
  if (currentLoggedInUser.value !== null) userId.value = currentLoggedInUser.value.id
})
</script>

<template>
  <div>
    <h1>{{ isNew ? 'Add new project' : 'Edit project' }}</h1>

    <div v-if="currentLoggedInUser === null">Loading...</div>
    <form @submit.prevent="submitForm" v-else>
      <label for="name"
        >Name
        <input type="text" id="name" v-model="form.name" autofocus required />
      </label>

      <label for="description"
        >Description
        <input
          type="text"
          id="description"
          v-model="form.description"
          required
        />
      </label>

      <label for="created-by" style="display: none;"
        >Created by
        <input
          type="number"
          id="created-by"
          v-model="userId"
          required
        />
      </label>

      <button type="submit">{{ isNew ? 'Add' : 'Save changes' }}</button>
    </form>
  </div>
</template>
