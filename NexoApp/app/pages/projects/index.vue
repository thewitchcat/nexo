<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

type Project = {
  id: number
  name: string
  description: string
  createdBy: number
}

const { $api } = useNuxtApp()
const router = useRouter()

const projects = ref<Project[]>([])
const isDeleteDialogVisible = ref(false)
const selectedProject = ref<Project | null>(null)

const addProject = () => {
  router.push('/projects/new')
}

const editProject = (id: number) => {
  router.push(`/projects/${id}`)
}

const openDeleteDialog = (project: Project) => {
  selectedProject.value = project
  isDeleteDialogVisible.value = true
}

const deleteProject = async () => {
  if (!selectedProject.value) return

  try {
    await $api.delete(`/projects/${selectedProject.value.id}`)

    selectedProject.value = null
    await loadProjects()
  } catch (e) {
    console.error(e)
  }
}

const loadProjects = async () => {
  try {
    const { data } = await $api.get('/projects')
    projects.value = data
  } catch (e) {
    console.error(e)
  }
}

onMounted(async () => {
  await loadProjects()
})
</script>

<template>
  <div>
    <h1>Project List</h1>
    <button @click="addProject">Add new project</button>
    <table>
      <thead>
        <tr>
          <th>Name</th>
          <th>Description</th>
          <th>Created by</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody v-if="projects.length === 0">
        <tr>
          <td colspan="4" style="text-align: center">Loading...</td>
        </tr>
      </tbody>
      <tbody v-else>
        <tr v-for="p in projects" :key="p.id">
          <td>{{ p.name }}</td>
          <td>{{ p.description }}</td>
          <td>{{ p.createdBy }}</td>
          <td>
            <button @click="editProject(p.id)">Edit</button>
            <button @click="openDeleteDialog(p)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>

    <DeleteDialog
      v-model="isDeleteDialogVisible"
      message="Are you sure want to delete this project and all with its tasks?"
      @confirm="deleteProject"
    ></DeleteDialog>
  </div>
</template>
