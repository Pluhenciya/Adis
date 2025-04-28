export interface Project {
    idProduct: number;
    name: string;
    status: 'draft' | 'inProgress' | 'completed' | 'overdue';
    description: string;
    budget: number;
    startDate?: Date;
    endDate?: Date;
    createdAt: Date;
  }