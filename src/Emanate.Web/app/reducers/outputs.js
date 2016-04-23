import {List, Map} from 'immutable';
import * as types from '../actions/outputs';

const initialState = List();

export default function outputs(state = initialState, action) {
  let outputIndex;

  switch (action.type) {
    case types.CREATE_OUTPUT:
      return state.push(Map(action.output));

    case types.UPDATE_OUTPUT:
      outputIndex = state.findIndex(output => output.get('id') === action.id);

      if(outputIndex < 0) {
        return state;
      }

      return state.mergeIn([outputIndex], action)

    case types.DELETE_OUTPUT:
      outputIndex = state.findIndex(output => output.get('id') === action.id);

      if(outputIndex < 0) {
        return state;
      }

      return state.delete(outputIndex);

    default:
      return state;
  }
}
