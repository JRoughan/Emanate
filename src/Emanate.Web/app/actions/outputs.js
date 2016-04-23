import uuid from 'node-uuid';

export const CREATE_OUTPUT = 'CREATE_OUTPUT';
export function createOutput(output) {
  return {
    type: CREATE_OUTPUT,
    output: {
      id: uuid.v4(),
      ...output
    }
  };
}

export const UPDATE_OUTPUT = 'UPDATE_OUTPUT';
export function updateOutput(updatedOutput) {
  return {
    type: UPDATE_OUTPUT,
    ...updatedOutput
  };
}

export const DELETE_OUTPUT = 'DELETE_OUTPUT';
export function deleteOutput(id) {
  return {
    type: DELETE_OUTPUT,
    id
  };
}
