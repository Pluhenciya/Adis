export interface Project {
    id: number;
    name: string;
    status: 'draft' | 'inProgress' | 'completed' | 'overdue';
    createdAt: Date;
    budget: number;
    startDate?: Date;
    endDate?: Date;
  }