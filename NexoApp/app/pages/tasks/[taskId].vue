<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

type Project = {
  id: number
  name: string
  description: string
}

type User = {
  id: number
  name: string
  email: string
  role: string
}

type Task = {
  id: number
  title: string
  description: string
  type: string
  status: string
  priority: string
  dueDate: string
  projectId: number
  assignedTo: number
}

type TaskForm = Omit<Task, 'id'>

const { $api } = useNuxtApp()

const router = useRouter()
const route = useRoute()

const projects = ref<Project[] | null>(null)
const loadProjects = async () => {
  try {
    const { data } = await $api.get('/projects')
    projects.value = data
  } catch (e) {
    console.error(e)
  }
}

const users = ref<User[] | null>(null)
const loadUsers = async () => {
  try {
    const { data } = await $api.get('/users?role=employee')
    users.value = data
  } catch (e) {
    console.error(e)
  }
}

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
const isNew = route.params.taskId === 'new'
const form = ref<TaskForm>({
  title: '',
  description: '',
  type: '',
  status: '',
  priority: '',
  dueDate: '',
  projectId: 0,
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
      createdBy: userId.value,
      assignedTo: form.value.assignedTo,
    }) : await $api.put(`/tasks/${route.params.taskId}`, {
      title: form.value.title,
      description: form.value.description,
      type: form.value.type,
      status: form.value.status,
      priority: form.value.priority,
      dueDate: form.value.dueDate,
      projectId: form.value.projectId,
      createdBy: userId.value,
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
      assignedTo: 0,
    }

    userId.value = 0

    router.push('/tasks')
  } catch (e) {
    console.error(e)
  }
}

if (!isNew) {
  const { data } = await $api.get(`/tasks/${route.params.taskId}`)
  console.log(data)

  if (data) {
    form.value = {
      title: data.title ?? '',
      description: data.description ?? '',
      type: data.type ?? '',
      status: data.status ?? '',
      priority: data.priority ?? '',
      dueDate: data.dueDate ?? '',
      projectId: data.projectId ?? 0,
      assignedTo: data.assignedTo ?? 0,
    }

    userId.value = data.createdBy ?? 0
  }
}

loadCurentLoggedInUser();
loadProjects()
loadUsers()
onMounted(() => {
  if (currentLoggedInUser.value !== null) userId.value = currentLoggedInUser.value.id
})
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
        <!-- <input
          type="number"
          id="project"
          v-model="form.projectId"
          required
        /> -->
        <select id="project" v-model="form.projectId">
          <option value="0" selected disabled>-- Select project --</option>
          <template v-for="project in projects" :key="project">
             <option :value="project.id">{{ project.name }}</option>
          </template>
        </select>
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

      <label for="assigned-to"
        >Assigned to
        <!-- <input
          type="number"
          id="assigned-to"
          v-model="form.assignedTo"
          required
        /> -->
        <select id="assigned-to" v-model="form.assignedTo">
          <option value="0" selected disabled>-- Select Employee --</option>
          <template v-for="user in users" :key="user">
            <option :value="user.id">{{ user.name }}</option>
          </template>
        </select>
      </label>

      <button type="submit">{{ isNew ? 'Add' : 'Save changes' }}</button>
    </form>
  </div>
</template>
