import { getGreeting } from '../support/app.po';

describe('annotation', () => {
  beforeEach(() => cy.visit('/'));

  it('should display welcome message', () => {
    getGreeting().contains('Welcome to annotation!');
  });
});
