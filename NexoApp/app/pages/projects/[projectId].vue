<script setup lang="ts">
type Project = {
  id: number
  name: string
  description: string
  createdBy: number
}

type ProjectForm = Omit<Project, 'id'>

const router = useRouter()
const route = useRoute()

const isNew = route.params.projectId === 'new'
const form = ref<ProjectForm>({
  name: '',
  description: '',
  createdBy: 0,
})

const submitForm = async () => {
  const url = isNew
    ? 'http://localhost:5015/projects'
    : `http://localhost:5015/projects/${route.params.projectId}`

  try {
    await $fetch(url, {
      method: isNew ? 'POST' : 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(form.value),
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

// ------------------ //

if (!isNew) {
  const { data, error } = await useFetch<Project>(
    `http://localhost:5015/projects/${route.params.projectId}`,
  )
  if (error.value) {
    console.error(error.value)
  }

  if (data.value) {
    form.value = {
      name: data.value.name ?? '',
      description: data.value.description ?? '',
      createdBy: data.value.createdBy ?? 0,
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
