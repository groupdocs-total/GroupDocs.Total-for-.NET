module.exports = {
  name: 'annotation',
  preset: '../../jest.config.js',
  coverageDirectory: '../../coverage/apps/annotation',
  snapshotSerializers: [
    'jest-preset-angular/AngularSnapshotSerializer.js',
    'jest-preset-angular/HTMLCommentSerializer.js'
  ]
};
