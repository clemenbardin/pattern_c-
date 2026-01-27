import api from '../config/api';

export const documentService = {
  generate: (documentType, clientType) =>
    api.post('/api/documents/generate', { documentType, clientType }),
};
