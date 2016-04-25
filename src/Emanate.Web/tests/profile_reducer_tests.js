import assert from 'assert';
import * as types from 'app/actions/profiles';
import reducer from 'app/reducers/profiles';

describe('ProfileReducer', () => {
  it('should return the initial state', () => {
    assert.equal(reducer(undefined, {}).count(), 0);
  });

  it('should create profiles', () => {
    const profile = {
      id: 'foobar',
      name: 'test'
    };

    assert.deepEqual(reducer(undefined, {
      type: types.CREATE_PROFILE,
      profile: profile
    }).toJS(), [profile]);
  });

  it('should update profiles', () => {
    const profile = {
      id: 'foobar',
      name: 'test'
    };
    const updatedName = 'foofoo';

    const profiles = reducer(undefined, {
      type: types.CREATE_PROFILE,
      profile: profile
    });
    const state = reducer(profiles, {
      type: types.UPDATE_PROFILE,
      id: profile.id,
      name: updatedName
    });

    assert.equal(state.count(), 1);
    assert.equal(state.get(0).get('id'), profile.id);
    assert.equal(state.get(0).get('name'), updatedName);
  });

  it('should not crash while updating a non-existent profile', () => {
    const profile = {
      id: 'foobar',
      name: 'test'
    };

    const profiles = reducer(undefined, {
      type: types.CREATE_PROFILE,
      profile: profile
    });
    const state = reducer(profiles, {
      type: types.UPDATE_PROFILE,
      id: profile.id + profile.id,
      name: 'foo'
    });

    assert.equal(state.count(), 1);
    assert.equal(state.get(0).get('id'), profile.id);
    assert.equal(state.get(0).get('name'), profile.name);
  });

  it('should delete profiles', () => {
    const profile = {
      id: 'foobar',
      name: 'test'
    };

    const profiles = reducer(undefined, {
      type: types.CREATE_PROFILE,
      profile: profile
    });
    const state = reducer(profiles, {
      type: types.DELETE_PROFILE,
      id: profile.id
    });

    assert.equal(state.count(), 0);
  });

  it('should not crash while deleting a non-existent profile', () => {
    const profile = {
      id: 'foobar',
      name: 'test'
    };

    const profiles = reducer(undefined, {
      type: types.CREATE_PROFILE,
      profile: profile
    });
    const state = reducer(profiles, {
      type: types.DELETE_PROFILE,
      id: profile.id + profile.id
    });

    assert.equal(state.count(), 1);
    assert.equal(state.get(0).get('id'), profile.id);
    assert.equal(state.get(0).get('name'), profile.name);
  });
});
