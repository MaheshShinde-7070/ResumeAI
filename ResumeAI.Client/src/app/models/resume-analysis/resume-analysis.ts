export interface AIAnalysisResult {
  overallScore: number;
  skills: string[];
  missingSkills: string[];
  suggestions: string[];
}

export interface ResumeUploadResponse {
  message: string;
  fileName: string;
  analysis: AIAnalysisResult;
}