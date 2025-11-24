import SignIn from '@/pages/auth/SignIn.vue'
import SignUp from '@/pages/auth/SignUp.vue'
import Dashboard from '@/pages/dashboard/Dashboard.vue'
import ProjectList from '@/pages/project/ProjectList.vue'
import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: Dashboard,
    },
    {
      path: '/signin',
      component: SignIn,
    },
    {
      path: '/signup',
      component: SignUp,
    },
    {
      path: '/projects',
      component: ProjectList,
    },
  ],
})

export default router
