import {List, Map} from 'immutable';
import uuid from 'node-uuid';

export const CREATE_OUTPUTGROUP = 'CREATE_OUTPUTGROUP';
export function createOutputGroup(outputGroup) {
  return {
    type: CREATE_OUTPUTGROUP,
    outputGroup: Map({
      id: uuid.v4(),
      outputs: List(outputGroup.outputs || []),
      ...outputGroup
    })
  };
};

export const UPDATE_OUTPUTGROUP = 'UPDATE_OUTPUTGROUP';
export function updateOutputGroup(updatedOutputGroup) {
  return {
    type: UPDATE_OUTPUTGROUP,
    ...updatedOutputGroup
  };
};

export const DELETE_OUTPUTGROUP = 'DELETE_OUTPUTGROUP';
export function deleteOutputGroup(id) {
  return {
    type: DELETE_OUTPUTGROUP,
    id
  };
};

export const ATTACH_TO_OUTPUTGROUP = 'ATTACH_TO_OUTPUTGROUP';
export function attachToOutputGroup(outputGroupId, outputId) {
  return {
    type: ATTACH_TO_OUTPUTGROUP,
    outputGroupId,
    outputId
  };
};

export const DETACH_FROM_OUTPUTGROUP = 'DETACH_FROM_OUTPUTGROUP';
export function detachFromOutputGroup(outputGroupId, outputId) {
  return {
    type: DETACH_FROM_OUTPUTGROUP,
    outputGroupId,
    outputId
  };
};

