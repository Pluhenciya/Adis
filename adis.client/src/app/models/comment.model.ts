export interface CommentDto{
    idComment: number;
    text: string;
    idSender: number;
    fullNameSender: string;
    createdAt: Date;
}

export interface PostCommentDto{
    idTask: number;
    text: string;
}
