import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResumeAnalysis } from './resume-analysis';

describe('ResumeAnalysis', () => {
  let component: ResumeAnalysis;
  let fixture: ComponentFixture<ResumeAnalysis>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResumeAnalysis],
    }).compileComponents();

    fixture = TestBed.createComponent(ResumeAnalysis);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
