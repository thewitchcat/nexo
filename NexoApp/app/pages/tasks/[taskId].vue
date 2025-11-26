<script setup lang="ts">
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
  const url = isNew
    ? 'http://localhost:5015/tasks'
    : `http://localhost:5015/tasks/${route.params.taskId}`

  try {
    await $fetch(url, {
      method: isNew ? 'POST' : 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(form.value),
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

// ------------------ //

if (!isNew) {
  const { data, error } = await useFetch<Task>(
    `http://localhost:5015/tasks/${route.params.taskId}`,
  )
  if (error.value) {
    console.error(error.value)
  }

  if (data.value) {
    form.value = {
      title: data.value.title ?? '',
      description: data.value.description ?? '',
      type: data.value.type ?? '',
      status: data.value.status ?? '',
      priority: data.value.priority ?? '',
      dueDate: data.value.dueDate ?? '',
      projectId: data.value.projectId ?? 0,
      createdBy: data.value.createdBy ?? 0,
      assignedTo: data.value.assignedTo ?? 0,
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
