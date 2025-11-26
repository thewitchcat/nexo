<script setup lang="ts">
type Project = {
  id: number
  name: string
  description: string
  createdBy: number
}

type ProjectForm = Omit<Project, 'id'>

const { $api } = useNuxtApp()

const router = useRouter()
const route = useRoute()

const isNew = route.params.projectId === 'new'
const form = ref<ProjectForm>({
  name: '',
  description: '',
  createdBy: 0,
})

const submitForm = async () => {
  try {
    isNew ? await $api.post('/projects', { 
      name: form.value.name,
      description: form.value.description,
      createdBy: form.value.createdBy,
    }) : await $api.put(`/projects/${route.params.projectId}`, {
      name: form.value.name,
      description: form.value.description,
      createdBy: form.value.createdBy,
    })

    form.value = {
      name: '',
      description: '',
      createdBy: 0,
    }

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
      createdBy: data.createdBy ?? 0,
    }
  }
}
</script>

<template>
  <div>
    <h1>{{ isNew ? 'Add new project' : 'Edit project' }}</h1>

    <form @submit.prevent="submitForm">
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

      <label for="created-by"
        >Created by
        <input
          type="number"
          id="created-by"
          v-model="form.createdBy"
          required
        />
      </label>

      <button type="submit">{{ isNew ? 'Add' : 'Save changes' }}</button>
    </form>
  </div>
</template>
