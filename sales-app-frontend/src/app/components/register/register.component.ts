import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styles: []
})
export class RegisterComponent {
  registerForm: FormGroup;
  isSubmitting = false;
  errorMessage = '';
  showPassword = false;
  passwordStrength = 0;
  passwordStrengthText = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required, 
        Validators.minLength(8),
        this.passwordStrengthValidator
      ]],
      confirmPassword: ['', [Validators.required]]
    }, { 
      validators: this.passwordMatchValidator 
    });

    this.registerForm.get('password')?.valueChanges.subscribe(password => {
      this.updatePasswordStrength(password);
    });
  }

  hasUpperCase(password: string | null | undefined): boolean {
    if (!password) return false;
    return /[A-Z]/.test(password);
  }

  hasLowerCase(password: string | null | undefined): boolean {
    if (!password) return false;
    return /[a-z]/.test(password);
  }

  hasNumber(password: string | null | undefined): boolean {
    if (!password) return false;
    return /[0-9]/.test(password);
  }

  hasSpecialChar(password: string | null | undefined): boolean {
    if (!password) return false;
    return /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password);
  }

  passwordStrengthValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.value;
    if (!password) return null;
    
    const hasUpperCase = /[A-Z]/.test(password);
    const hasLowerCase = /[a-z]/.test(password);
    const hasNumeric = /[0-9]/.test(password);
    const hasSpecialChar = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password);
    
    const passwordValid = hasUpperCase && hasLowerCase && hasNumeric && hasSpecialChar;
    
    return passwordValid ? null : {
      passwordStrength: {
        hasUpperCase,
        hasLowerCase,
        hasNumeric,
        hasSpecialChar
      }
    };
  }

  updatePasswordStrength(password: string): void {
    if (!password) {
      this.passwordStrength = 0;
      this.passwordStrengthText = '';
      return;
    }
    
    let strength = 0;
    
    if (password.length >= 8) strength += 1;
    if (password.length >= 12) strength += 1;
    
    if (this.hasUpperCase(password)) strength += 1;
    if (this.hasLowerCase(password)) strength += 1;
    if (this.hasNumber(password)) strength += 1;
    if (this.hasSpecialChar(password)) strength += 1;
    
    this.passwordStrength = Math.min(100, (strength / 6) * 100);
    
    if (this.passwordStrength < 33) {
      this.passwordStrengthText = 'Fraca';
    } else if (this.passwordStrength < 66) {
      this.passwordStrengthText = 'Média';
    } else {
      this.passwordStrengthText = 'Forte';
    }
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      form.get('confirmPassword')?.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    
    return null;
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const { name, email, password } = this.registerForm.value;

    this.authService.register({ name, email, password }).subscribe({
      next: () => {
        this.router.navigate(['/orders']);
      },
      error: (error) => {
        console.error('Registration error', error);
        this.errorMessage = error.error?.message || 'Falha no registro. Tente novamente.';
        this.isSubmitting = false;
      }
    });
  }
}