import {combineReducers} from 'redux';
import outputGroups from './outputGroups';
import outputs from './outputs';
import profiles from './profiles';

export default combineReducers({
  outputGroups,
  outputs,
  profiles
});
