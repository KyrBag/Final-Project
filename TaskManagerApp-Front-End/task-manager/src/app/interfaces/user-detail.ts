import { UserTasks } from "./user-tasks";

export interface UserDetail {
   
  username: string;
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  tasks: UserTasks[];
}