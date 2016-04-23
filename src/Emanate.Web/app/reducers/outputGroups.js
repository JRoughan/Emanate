import {List, Map} from 'immutable';
import update from 'react-addons-update';
import * as types from '../actions/outputGroups';

const initialState = List();

export default function outputGroups(state = initialState, action) {
  let outputGroupIndex;

  switch (action.type) {
    case types.CREATE_OUTPUTGROUP:
      return state.push(Map(action.outputGroup));

    case types.UPDATE_OUTPUTGROUP:
      outputGroupIndex = state.findIndex(outputGroup => outputGroup.get('id') === action.id);

      if(outputGroupIndex < 0) {
        return state;
      }

      return state.mergeIn([outputGroupIndex], action);

    case types.DELETE_OUTPUTGROUP:
      outputGroupIndex = state.findIndex(outputGroup => outputGroup.get('id') === action.id);

      if(outputGroupIndex < 0) {
        return state;
      }

      return state.delete(outputGroupIndex);

    case types.ATTACH_TO_OUTPUTGROUP:
      const outputGroupId = action.outputGroupId;
      const outputId = action.outputId;

      return state.map(
        outputGroup => {
          const outputIndex = outputGroup.get('outputs').indexOf(outputId);

          // Delete outputs if found
          if(outputIndex >= 0) {
             return outputGroup.deleteIn(['outputs', outputIndex]);
          }

          // Attach output to the outputGroup
          if(outputGroup.get('id') === outputGroupId) {
            return outputGroup.setIn(['outputs'], outputGroup.get('outputs').push(outputId));
          }

          return outputGroup;
        }
      );

    case types.DETACH_FROM_OUTPUTGROUP:
      return state.updateIn(
        [state.findIndex(outputGroup => outputGroup.id === action.outputGroupId)],
        outputGroup => outputGroup.deleteIn(['outputs', outputGroup.get('outputs').indexOf(action.outputId)])
      );

    default:
      return state;
  }
}
