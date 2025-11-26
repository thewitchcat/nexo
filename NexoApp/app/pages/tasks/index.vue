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

const { $api } = useNuxtApp()
const router = useRouter()

const tasks = ref<Task[]>([])
const isDeleteDialogVisible = ref(false)
const selectedTask = ref<Task | null>(null)

const addTask = () => {
  router.push('/tasks/new')
}

const editTask = (id: number) => {
  router.push(`/tasks/${id}`)
}

const openDeleteDialog = (task: Task) => {
  selectedTask.value = task
  isDeleteDialogVisible.value = true
}

const deleteTask = async () => {
  if (!selectedTask.value) return

  try {
    await $api.delete(`/tasks/${selectedTask.value.id}`)

    selectedTask.value = null
    await loadTasks()
  } catch (e) {
    console.error(e)
  }
}

const loadTasks = async () => {
  try {
    const { data } = await $api.get('/tasks')
    tasks.value = data
  } catch (e) {
    console.error(e)
  }
}

onMounted(async () => {
  await loadTasks()
})
</script>

<template>
  <div>
    <h1>Task List</h1>
    <button @click="addTask">Add new task</button>
    <table>
      <thead>
        <tr>
          <th>Title</th>
          <th>Description</th>
          <th>Type</th>
          <th>Status</th>
          <th>Priority</th>
          <th>Due date</th>
          <th>Project</th>
          <th>Created by</th>
          <th>Assigned to</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody v-if="tasks.length === 0">
        <tr>
          <td colspan="4" style="text-align: center">Loading...</td>
        </tr>
      </tbody>
      <tbody v-else>
        <tr v-for="t in tasks" :key="t.id">
          <td>{{ t.title }}</td>
          <td>{{ t.description }}</td>
          <td>{{ t.type }}</td>
          <td>{{ t.status }}</td>
          <td>{{ t.priority }}</td>
          <td>{{ t.dueDate }}</td>
          <td>{{ t.projectId }}</td>
          <td>{{ t.createdBy }}</td>
          <td>{{ t.assignedTo }}</td>
          <td>
            <button @click="editTask(t.id)">Edit</button>
            <button @click="openDeleteDialog(t)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>

    <DeleteDialog
      v-model="isDeleteDialogVisible"
      message="Are you sure want to delete this task?"
      @confirm="deleteTask"
    ></DeleteDialog>
  </div>
</template>
