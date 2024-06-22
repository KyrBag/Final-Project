export interface EditTaskRequest {
    id: string;
  title: string;
  description: string;
  dueDate: Date;
  isCompleted: boolean;
  username: string ;
}