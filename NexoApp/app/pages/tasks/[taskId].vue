<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

type Task = {
  id: number
  title: string
  description: string
  type: string
  status: string
  priority: string
  dueDate: string
  projectId: number
  createdBy: number
  assignedTo: number
}

type TaskForm = Omit<Task, 'id'>

const { $api } = useNuxtApp()

const router = useRouter()
const route = useRoute()

const isNew = route.params.taskId === 'new'
const form = ref<TaskForm>({
  title: '',
  description: '',
  type: '',
  status: '',
  priority: '',
  dueDate: '',
  projectId: 0,
  createdBy: 0,
  assignedTo: 0,
})

const submitForm = async () => {
  try {
    isNew ? await $api.post('/tasks', {
      title: form.value.title,
      description: form.value.description,
      type: form.value.type,
      status: form.value.status,
      priority: form.value.priority,
      dueDate: form.value.dueDate,
      projectId: form.value.projectId,
      createdBy: form.value.createdBy,
      assignedTo: form.value.assignedTo,
    }) : await $api.put(`/tasks/${route.params.taskId}`, {
      title: form.value.title,
      description: form.value.description,
      type: form.value.type,
      status: form.value.status,
      priority: form.value.priority,
      dueDate: form.value.dueDate,
      projectId: form.value.projectId,
      createdBy: form.value.createdBy,
      assignedTo: form.value.assignedTo,
    })

    form.value = {
      title: '',
      description: '',
      type: '',
      status: '',
      priority: '',
      dueDate: '',
      projectId: 0,
      createdBy: 0,
      assignedTo: 0,
    }

    router.push('/tasks')
  } catch (e) {
    console.error(e)
  }
}

if (!isNew) {
  const { data } = await $api.get(`/tasks/${route.params.taskId}`)

  if (data) {
    form.value = {
      title: data.title ?? '',
      description: data.description ?? '',
      type: data.type ?? '',
      status: data.status ?? '',
      priority: data.priority ?? '',
      dueDate: data.dueDate ?? '',
      projectId: data.projectId ?? 0,
      createdBy: data.createdBy ?? 0,
      assignedTo: data.assignedTo ?? 0,
    }
  }
}
</script>

<template>
  <div>
    <h1>{{ isNew ? 'Add new task' : 'Edit task' }}</h1>

    <form @submit.prevent="submitForm">
      <label for="title"
        >Title
        <input type="text" id="title" v-model="form.title" autofocus required />
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

      <label for="type"
        >Type
        <input
          type="text"
          id="type"
          v-model="form.type"
          required
        />
      </label>

      <label for="status"
        >Status
        <input
          type="text"
          id="status"
          v-model="form.status"
          required
        />
      </label>

      <label for="priority"
        >Priority
        <input
          type="text"
          id="priority"
          v-model="form.priority"
          required
        />
      </label>

      <label for="due-date"
        >Due date
        <input
          type="date"
          id="due-date"
          v-model="form.dueDate"
          required
        />
      </label>

      <label for="project"
        >Project
        <input
          type="number"
          id="project"
          v-model="form.projectId"
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

      <label for="assigned-to"
        >Assigned to
        <input
          type="number"
          id="assigned-to"
          v-model="form.assignedTo"
          required
        />
      </label>

      <button type="submit">{{ isNew ? 'Add' : 'Save changes' }}</button>
    </form>
  </div>
</template>
