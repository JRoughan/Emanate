import {List, Map} from 'immutable';
import * as types from '../actions/profiles';

const initialState = List();

export default function profiles(state = initialState, action) {
  let profileIndex;

  switch (action.type) {
    case types.CREATE_PROFILE:
      return state.push(Map(action.profile));

    case types.UPDATE_PROFILE:
      profileIndex = state.findIndex(profile => profile.get('id') === action.id);

      if(profileIndex < 0) {
        return state;
      }

      return state.mergeIn([profileIndex], action);

    case types.DELETE_PROFILE:
      profileIndex = state.findIndex(profile => profile.get('id') === action.id);

      if(profileIndex < 0) {
        return state;
      }

      return state.delete(profileIndex);

    case types.ATTACH_TO_PROFILE:
      const profileId = action.profileId;
      const stateId = action.stateId;

      return state.map(
        profile => {
          const stateIndex = profile.get('states').indexOf(stateId);

          // Delete states if found
          if(stateIndex >= 0) {
            return profile.deleteIn(['outputs', stateIndex]);
          }

          // Attach state to the profile
          if(profile.get('id') === profileId) {
            return profile.setIn(['states'], profile.get('states').push(stateId));
          }

          return profile;
        }
      );

    case types.DETACH_FROM_PROFILE:
      return state.updateIn(
        [state.findIndex(profile => profile.id === action.profileId)],
        profile => profile.deleteIn(['states', profile.get('states').indexOf(action.stateId)])
      );

    default:
      return state;
  }
}
