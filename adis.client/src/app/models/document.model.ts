export interface DocumentDto {
    idDocument: number;
    fileName: string;
    idUser?: number;
    documentType: DocumentType;
}

export enum DocumentType {
    GOST = 'ГОСТ',
    SNIP = 'СНиП',
    SP = 'СП',
    TU = 'ТУ',
    SanPin = 'СанПиН',
    Estimate = 'Смета',
    TechnicalRegulation = 'Технический регламент',
    Other = 'Другое'
  }