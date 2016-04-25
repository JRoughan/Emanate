import {List, Map} from 'immutable';
import uuid from 'node-uuid';

export const CREATE_PROFILE = 'CREATE_PROFILE';
export function createProfile(profile) {
  return {
    type: CREATE_PROFILE,
    profile: Map({
      id: uuid.v4(),
      name: Math.random().toString(36).substr(2, 9),
      states: List(profile.states || []),
      ...profile
    })
  };
}

export const UPDATE_PROFILE = 'UPDATE_PROFILE';
export function updateProfile(updatedProfile) {
  return {
    type: UPDATE_PROFILE,
    ...updatedProfile
  };
}

export const DELETE_PROFILE = 'DELETE_PROFILE';
export function deleteProfile(id) {
  return {
    type: DELETE_PROFILE,
    id
  };
}

export const ATTACH_TO_PROFILE = 'ATTACH_TO_PROFILE';
export function attachToProfile(profileId, stateId) {
  return {
    type: ATTACH_TO_PROFILE,
    profileId,
    stateId
  };
}

export const DETACH_FROM_PROFILE = 'DETACH_FROM_PROFILE';
export function detachFromProfile(profileId, stateId) {
  return {
    type: DETACH_FROM_PROFILE,
    profileId,
    stateId
  };
}

